```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-ERJJUF : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **24.94 ms** |  **2.464 ms** |  **1.466 ms** |  **1.00** |    **0.08** |   **2794.9 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  12.48 ms |  2.654 ms |  1.756 ms |  0.50 |    0.07 |   477.17 KB |        0.17 |
| GaroaBulk      | 1000  |  12.31 ms |  0.763 ms |  0.504 ms |  0.50 |    0.03 |   111.56 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **150.00 ms** | **16.234 ms** | **10.738 ms** |  **1.00** |    **0.10** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 |  93.94 ms | 14.564 ms |  8.667 ms |  0.63 |    0.07 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  89.38 ms |  4.958 ms |  2.950 ms |  0.60 |    0.05 |  1060.78 KB |        0.04 |
