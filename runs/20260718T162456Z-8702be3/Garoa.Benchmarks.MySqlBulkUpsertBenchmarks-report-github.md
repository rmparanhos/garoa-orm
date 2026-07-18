```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2
  Job-CXNUMO : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated  | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|-----------:|------------:|
| **Dapper**        | **1000**  |  **25.41 ms** | **10.126 ms** |  **6.698 ms** |  **1.07** |    **0.39** | **2912.86 KB** |        **1.00** |
| ManualStaging | 1000  |  14.27 ms |  2.003 ms |  1.325 ms |  0.60 |    0.16 |  478.86 KB |        0.16 |
| GaroaBulk     | 1000  |  12.23 ms |  1.062 ms |  0.702 ms |  0.51 |    0.13 |  118.27 KB |        0.04 |
|               |       |           |           |           |       |         |            |             |
| **Dapper**        | **10000** | **119.37 ms** | **24.476 ms** | **16.189 ms** |  **1.02** |    **0.18** | **27944.3 KB** |        **1.00** |
| ManualStaging | 10000 | 101.29 ms | 13.780 ms |  9.115 ms |  0.86 |    0.13 | 5262.36 KB |        0.19 |
| GaroaBulk     | 10000 |  89.97 ms | 15.767 ms | 10.429 ms |  0.77 |    0.13 | 1067.49 KB |        0.04 |
