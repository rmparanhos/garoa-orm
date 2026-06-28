```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-MQVEKV : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated  | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|-----------:|------------:|
| **Dapper**        | **1000**  |  **26.32 ms** | **10.102 ms** |  **6.682 ms** |  **1.05** |    **0.35** | **2795.67 KB** |        **1.00** |
| ManualStaging | 1000  |  14.64 ms |  2.345 ms |  1.551 ms |  0.59 |    0.14 |  478.86 KB |        0.17 |
| GaroaBulk     | 1000  |  13.80 ms |  2.686 ms |  1.776 ms |  0.55 |    0.14 |  118.05 KB |        0.04 |
|               |       |           |           |           |       |         |            |             |
| **Dapper**        | **10000** | **120.30 ms** | **17.082 ms** | **11.299 ms** |  **1.01** |    **0.13** | **27944.3 KB** |        **1.00** |
| ManualStaging | 10000 | 107.39 ms | 19.194 ms | 12.695 ms |  0.90 |    0.13 | 5262.36 KB |        0.19 |
| GaroaBulk     | 10000 |  94.92 ms | 20.235 ms | 13.384 ms |  0.80 |    0.13 | 1067.28 KB |        0.04 |
