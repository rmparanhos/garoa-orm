```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-MQVEKV : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **27.74 ms** |  **9.946 ms** |  **6.579 ms** |  **1.05** |    **0.32** |   **2794.9 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  11.53 ms |  1.639 ms |  0.976 ms |  0.43 |    0.09 |   477.17 KB |        0.17 |
| GaroaBulk      | 1000  |  10.24 ms |  0.626 ms |  0.373 ms |  0.39 |    0.08 |   111.56 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **150.27 ms** | **30.745 ms** | **20.336 ms** |  **1.02** |    **0.18** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 |  98.35 ms | 14.854 ms |  9.825 ms |  0.66 |    0.10 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  86.71 ms | 11.015 ms |  6.555 ms |  0.59 |    0.08 |  1060.78 KB |        0.04 |
