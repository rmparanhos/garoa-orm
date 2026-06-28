```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-ZONUUP : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **28.40 ms** | **10.252 ms** |  **6.781 ms** |  **1.05** |    **0.33** |  **2912.09 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  11.69 ms |  2.140 ms |  1.273 ms |  0.43 |    0.10 |   477.17 KB |        0.16 |
| GaroaBulk      | 1000  |  10.32 ms |  0.629 ms |  0.329 ms |  0.38 |    0.08 |   111.56 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **145.47 ms** | **16.174 ms** |  **9.625 ms** |  **1.00** |    **0.09** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 | 107.14 ms | 18.547 ms | 12.268 ms |  0.74 |    0.09 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  88.56 ms | 16.270 ms | 10.762 ms |  0.61 |    0.08 |  1060.78 KB |        0.04 |
