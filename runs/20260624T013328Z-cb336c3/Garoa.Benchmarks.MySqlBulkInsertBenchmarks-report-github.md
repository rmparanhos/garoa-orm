```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-BBFTZT : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **29.13 ms** | **10.065 ms** |  **6.657 ms** |  **1.04** |    **0.31** |  **2912.09 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  12.80 ms |  1.851 ms |  1.224 ms |  0.46 |    0.10 |   477.17 KB |        0.16 |
| GaroaBulk      | 1000  |  11.16 ms |  2.193 ms |  1.451 ms |  0.40 |    0.10 |   111.56 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **152.27 ms** | **24.719 ms** | **16.350 ms** |  **1.01** |    **0.14** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 |  94.35 ms | 13.434 ms |  7.994 ms |  0.63 |    0.08 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  85.95 ms |  8.706 ms |  5.181 ms |  0.57 |    0.06 |  1060.78 KB |        0.04 |
