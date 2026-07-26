```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2
  Job-TVLTUZ : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **32.03 ms** | **12.246 ms** |  **8.100 ms** |  **1.07** |    **0.39** |   **2794.9 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  12.90 ms |  1.049 ms |  0.624 ms |  0.43 |    0.12 |   477.17 KB |        0.17 |
| GaroaBulk      | 1000  |  11.23 ms |  0.621 ms |  0.370 ms |  0.37 |    0.10 |   111.45 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **167.20 ms** | **24.967 ms** | **16.514 ms** |  **1.01** |    **0.13** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 | 107.72 ms | 24.571 ms | 16.252 ms |  0.65 |    0.11 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  92.35 ms | 17.672 ms | 11.689 ms |  0.56 |    0.09 |  1060.66 KB |        0.04 |
