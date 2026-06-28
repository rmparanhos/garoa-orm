```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-ERJJUF : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated  | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|-----------:|------------:|
| **Dapper**        | **1000**  |  **25.94 ms** |  **9.454 ms** |  **6.253 ms** |  **1.05** |    **0.34** | **2912.86 KB** |        **1.00** |
| ManualStaging | 1000  |  14.86 ms |  2.458 ms |  1.463 ms |  0.60 |    0.14 |  478.86 KB |        0.16 |
| GaroaBulk     | 1000  |  12.91 ms |  0.585 ms |  0.306 ms |  0.52 |    0.11 |  118.05 KB |        0.04 |
|               |       |           |           |           |       |         |            |             |
| **Dapper**        | **10000** | **128.54 ms** | **32.635 ms** | **21.586 ms** |  **1.02** |    **0.22** | **27944.3 KB** |        **1.00** |
| ManualStaging | 10000 | 106.18 ms | 21.306 ms | 14.093 ms |  0.85 |    0.16 | 5262.36 KB |        0.19 |
| GaroaBulk     | 10000 |  98.28 ms | 12.835 ms |  7.638 ms |  0.78 |    0.13 | 1067.28 KB |        0.04 |
