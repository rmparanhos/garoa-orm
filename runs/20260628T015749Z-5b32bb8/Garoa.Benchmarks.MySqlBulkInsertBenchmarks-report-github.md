```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-DMGYAW : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **22.77 ms** |  **1.423 ms** |  **0.847 ms** |  **1.00** |    **0.05** |   **2794.9 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  11.93 ms |  1.829 ms |  1.089 ms |  0.52 |    0.05 |   477.17 KB |        0.17 |
| GaroaBulk      | 1000  |  10.45 ms |  0.513 ms |  0.305 ms |  0.46 |    0.02 |   111.56 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **145.54 ms** | **18.408 ms** | **12.176 ms** |  **1.01** |    **0.11** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 |  94.61 ms | 18.463 ms | 10.987 ms |  0.65 |    0.09 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  88.72 ms |  8.619 ms |  5.129 ms |  0.61 |    0.06 |  1060.78 KB |        0.04 |
