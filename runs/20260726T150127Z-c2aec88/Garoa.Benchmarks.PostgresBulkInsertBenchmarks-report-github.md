```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2
  Job-TVLTUZ : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **20.067 ms** |  **7.2404 ms** |  **4.7891 ms** |  **1.05** |    **0.34** |  **2772.71 KB** |       **1.000** |
| ManualCopy | 1000  |   6.987 ms |  0.2013 ms |  0.1198 ms |  0.37 |    0.08 |     2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   7.000 ms |  0.1860 ms |  0.0973 ms |  0.37 |    0.08 |     3.42 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **137.180 ms** | **77.9934 ms** | **51.5878 ms** |  **1.11** |    **0.51** | **26544.45 KB** |       **1.000** |
| ManualCopy | 10000 |  41.998 ms | 19.5605 ms | 11.6402 ms |  0.34 |    0.13 |      4.7 KB |       0.000 |
| GaroaBulk  | 10000 |  40.683 ms | 25.7988 ms | 15.3525 ms |  0.33 |    0.15 |     4.73 KB |       0.000 |
