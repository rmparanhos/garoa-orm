```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-SYXJIQ : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **28.02 ms** |  **9.655 ms** |  **6.386 ms** |  **1.04** |    **0.31** |  **2912.09 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  12.15 ms |  2.396 ms |  1.426 ms |  0.45 |    0.10 |   477.17 KB |        0.16 |
| GaroaBulk      | 1000  |  11.62 ms |  1.305 ms |  0.863 ms |  0.43 |    0.09 |   111.56 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **151.55 ms** | **22.569 ms** | **14.928 ms** |  **1.01** |    **0.13** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 | 100.36 ms | 19.739 ms | 11.746 ms |  0.67 |    0.09 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  89.04 ms | 11.620 ms |  6.915 ms |  0.59 |    0.07 |  1060.78 KB |        0.04 |
