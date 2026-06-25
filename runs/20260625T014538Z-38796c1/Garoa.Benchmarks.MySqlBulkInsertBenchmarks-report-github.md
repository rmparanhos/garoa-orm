```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-IUIATW : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **25.95 ms** |  **9.642 ms** |  **6.377 ms** |  **1.05** |    **0.34** |  **2912.09 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  10.21 ms |  0.829 ms |  0.433 ms |  0.41 |    0.09 |   477.17 KB |        0.16 |
| GaroaBulk      | 1000  |  10.62 ms |  1.957 ms |  1.295 ms |  0.43 |    0.11 |   111.56 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **152.75 ms** | **51.883 ms** | **30.875 ms** |  **1.03** |    **0.27** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 | 103.09 ms | 24.547 ms | 16.236 ms |  0.70 |    0.16 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  82.66 ms | 15.261 ms | 10.094 ms |  0.56 |    0.12 |  1060.78 KB |        0.04 |
