```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2
  Job-TVLTUZ : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated  | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|-----------:|------------:|
| **Dapper**        | **1000**  |  **21.08 ms** |  **1.253 ms** |  **0.746 ms** |  **1.00** |    **0.05** | **2795.67 KB** |        **1.00** |
| ManualStaging | 1000  |  18.07 ms |  2.063 ms |  1.364 ms |  0.86 |    0.07 |  478.86 KB |        0.17 |
| GaroaBulk     | 1000  |  14.47 ms |  1.721 ms |  0.900 ms |  0.69 |    0.05 |  118.27 KB |        0.04 |
|               |       |           |           |           |       |         |            |             |
| **Dapper**        | **10000** | **136.02 ms** | **23.553 ms** | **14.016 ms** |  **1.01** |    **0.14** | **27944.3 KB** |        **1.00** |
| ManualStaging | 10000 | 111.62 ms | 17.836 ms | 11.798 ms |  0.83 |    0.11 | 5262.36 KB |        0.19 |
| GaroaBulk     | 10000 |  96.41 ms | 12.729 ms |  6.658 ms |  0.72 |    0.08 | 1067.49 KB |        0.04 |
