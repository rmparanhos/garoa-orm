#!/usr/bin/env python3
"""Enforce the Garoa-vs-Dapper performance ratio from a BenchmarkDotNet JSON report.

Garoa competes against Dapper in the same run, with Dapper as the BenchmarkDotNet
[Baseline]. Runner noise affects both equally, so the per-parameter ratio (Garoa mean /
Dapper mean) is stable and is what we gate on. Exits non-zero if any ratio exceeds the
threshold, so a real regression (e.g. a broken mapper cache) fails CI.

Usage:
    check_threshold.py [results_dir] [--threshold N] [--baseline Dapper] [--candidate Garoa]
"""
import argparse
import glob
import json
import os
import sys

DEFAULT_THRESHOLD = 1.30


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("results_dir", nargs="?", default="BenchmarkDotNet.Artifacts/results")
    parser.add_argument("--threshold", type=float,
                        default=float(os.environ.get("GAROA_BENCH_THRESHOLD", DEFAULT_THRESHOLD)))
    parser.add_argument("--baseline", default="Dapper")
    parser.add_argument("--candidate", default="Garoa")
    args = parser.parse_args()

    reports = glob.glob(os.path.join(args.results_dir, "*-report-full-compressed.json"))
    if not reports:
        print(f"::error::No BenchmarkDotNet JSON report found in '{args.results_dir}'.")
        return 2

    means: dict[str, dict[str, float]] = {}
    for report in reports:
        with open(report) as handle:
            data = json.load(handle)
        for bench in data.get("Benchmarks", []):
            params = bench.get("Parameters", "") or "(none)"
            means.setdefault(params, {})[bench["Method"]] = bench["Statistics"]["Mean"]

    print(f"Threshold: {args.candidate} mean must be <= {args.threshold:.2f}x {args.baseline} mean\n")
    header = f"{'Params':<14}{args.baseline + ' (ns)':>16}{args.candidate + ' (ns)':>16}{'Ratio':>10}  Status"
    print(header)
    print("-" * len(header))

    failures = []
    for params in sorted(means):
        row = means[params]
        if args.baseline not in row or args.candidate not in row:
            continue
        baseline = row[args.baseline]
        candidate = row[args.candidate]
        ratio = candidate / baseline
        ok = ratio <= args.threshold
        status = "OK" if ok else "FAIL"
        print(f"{params:<14}{baseline:>16.1f}{candidate:>16.1f}{ratio:>10.3f}  {status}")
        if not ok:
            failures.append((params, ratio))

    print()
    if failures:
        for params, ratio in failures:
            print(f"::error::Performance regression at [{params}]: "
                  f"ratio {ratio:.3f} exceeds threshold {args.threshold:.2f}.")
        return 1

    print("All ratios within threshold.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
