```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V45, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-NVVPSZ : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Median    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **14.014 ms** |  **4.8044 ms** |  **3.1778 ms** | **13.840 ms** |  **1.05** |    **0.35** |  **2655.48 KB** |       **1.000** |
| ManualCopy | 1000  |   3.527 ms |  0.2076 ms |  0.1086 ms |  3.522 ms |  0.27 |    0.07 |     2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   3.791 ms |  0.8875 ms |  0.5870 ms |  3.541 ms |  0.29 |    0.08 |     3.24 KB |       0.001 |
|            |       |            |            |            |           |       |         |             |             |
| **Dapper**     | **10000** | **101.742 ms** | **92.1666 ms** | **60.9625 ms** | **70.984 ms** |  **1.31** |    **1.00** | **26542.58 KB** |       **1.000** |
| ManualCopy | 10000 |  21.902 ms |  5.8449 ms |  3.0570 ms | 21.640 ms |  0.28 |    0.13 |      4.6 KB |       0.000 |
| GaroaBulk  | 10000 |  37.692 ms | 18.8550 ms |  9.8615 ms | 36.226 ms |  0.48 |    0.25 |     5.26 KB |       0.000 |
