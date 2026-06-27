```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-WUFBPK : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **16.638 ms** |  **3.7925 ms** |  **2.2568 ms** |  **1.01** |    **0.18** |  **2772.71 KB** |       **1.000** |
| ManualCopy | 1000  |   6.695 ms |  0.1708 ms |  0.0893 ms |  0.41 |    0.05 |     1.79 KB |       0.001 |
| GaroaBulk  | 1000  |   6.630 ms |  0.1033 ms |  0.0540 ms |  0.40 |    0.05 |     3.57 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **135.969 ms** | **54.4856 ms** | **36.0388 ms** |  **1.06** |    **0.37** | **26543.52 KB** |       **1.000** |
| ManualCopy | 10000 |  43.312 ms | 25.4344 ms | 15.1356 ms |  0.34 |    0.14 |     4.04 KB |       0.000 |
| GaroaBulk  | 10000 |  46.374 ms | 25.1467 ms | 16.6330 ms |  0.36 |    0.15 |     4.27 KB |       0.000 |
