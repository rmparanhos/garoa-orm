```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-UVLIRA : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated  | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|-----------:|------------:|
| **Dapper**     | **1000**  |  **13.092 ms** |  **0.5391 ms** |  **0.2820 ms** |  **1.00** |    **0.03** | **2656.09 KB** |       **1.000** |
| ManualCopy | 1000  |   5.785 ms |  0.3549 ms |  0.2112 ms |  0.44 |    0.02 |    2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   5.756 ms |  0.1319 ms |  0.0785 ms |  0.44 |    0.01 |    3.57 KB |       0.001 |
|            |       |            |            |            |       |         |            |             |
| **Dapper**     | **10000** | **142.140 ms** | **61.3200 ms** | **40.5594 ms** |  **1.08** |    **0.42** | **26543.8 KB** |       **1.000** |
| ManualCopy | 10000 |  42.313 ms | 18.2917 ms | 12.0988 ms |  0.32 |    0.13 |    4.04 KB |       0.000 |
| GaroaBulk  | 10000 |  43.522 ms | 11.9835 ms |  7.1312 ms |  0.33 |    0.10 |    5.21 KB |       0.000 |
