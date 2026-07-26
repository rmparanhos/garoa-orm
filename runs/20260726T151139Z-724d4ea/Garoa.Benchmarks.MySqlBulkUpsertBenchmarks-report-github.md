```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2
  Job-LYONEC : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated  | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|-----------:|------------:|
| **Dapper**        | **1000**  |  **17.57 ms** |  **0.648 ms** |  **0.339 ms** |  **1.00** |    **0.03** | **2795.67 KB** |        **1.00** |
| ManualStaging | 1000  |  14.67 ms |  2.343 ms |  1.549 ms |  0.84 |    0.09 |  478.86 KB |        0.17 |
| GaroaBulk     | 1000  |  12.37 ms |  1.619 ms |  1.071 ms |  0.70 |    0.06 |  118.27 KB |        0.04 |
|               |       |           |           |           |       |         |            |             |
| **Dapper**        | **10000** | **110.70 ms** | **17.011 ms** | **10.123 ms** |  **1.01** |    **0.12** | **27944.3 KB** |        **1.00** |
| ManualStaging | 10000 |  95.27 ms | 21.156 ms | 13.993 ms |  0.87 |    0.14 | 5262.36 KB |        0.19 |
| GaroaBulk     | 10000 |  82.62 ms |  8.244 ms |  4.312 ms |  0.75 |    0.07 | 1067.49 KB |        0.04 |
