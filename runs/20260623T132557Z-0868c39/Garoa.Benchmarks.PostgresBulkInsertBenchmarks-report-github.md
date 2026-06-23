```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V45, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-MSPUSS : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean      | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **7.184 ms** |  **0.4118 ms** |  **0.2154 ms** |  **1.00** |    **0.04** |  **2655.52 KB** |       **1.000** |
| ManualCopy | 1000  |  3.324 ms |  0.1527 ms |  0.0909 ms |  0.46 |    0.02 |     1.92 KB |       0.001 |
| GaroaBulk  | 1000  |  3.510 ms |  0.6314 ms |  0.3302 ms |  0.49 |    0.05 |     3.51 KB |       0.001 |
|            |       |           |            |            |       |         |             |             |
| **Dapper**     | **10000** | **95.515 ms** | **47.9358 ms** | **31.7066 ms** |  **1.12** |    **0.56** | **26545.86 KB** |       **1.000** |
| ManualCopy | 10000 | 17.591 ms |  8.2915 ms |  4.9342 ms |  0.21 |    0.10 |     4.32 KB |       0.000 |
| GaroaBulk  | 10000 | 24.340 ms | 10.7343 ms |  6.3878 ms |  0.29 |    0.13 |     4.82 KB |       0.000 |
