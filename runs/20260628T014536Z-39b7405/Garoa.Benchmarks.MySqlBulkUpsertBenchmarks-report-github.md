```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-ERJCVT : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated  | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|-----------:|------------:|
| **Dapper**        | **1000**  |  **18.92 ms** |  **1.649 ms** |  **0.862 ms** |  **1.00** |    **0.06** | **2795.67 KB** |        **1.00** |
| ManualStaging | 1000  |  14.53 ms |  1.797 ms |  1.189 ms |  0.77 |    0.07 |  478.86 KB |        0.17 |
| GaroaBulk     | 1000  |  11.89 ms |  0.384 ms |  0.228 ms |  0.63 |    0.03 |  118.05 KB |        0.04 |
|               |       |           |           |           |       |         |            |             |
| **Dapper**        | **10000** | **132.40 ms** | **35.626 ms** | **23.564 ms** |  **1.03** |    **0.24** | **27944.3 KB** |        **1.00** |
| ManualStaging | 10000 | 101.58 ms | 16.457 ms | 10.885 ms |  0.79 |    0.15 | 5262.36 KB |        0.19 |
| GaroaBulk     | 10000 |  97.23 ms | 18.060 ms | 11.945 ms |  0.75 |    0.15 | 1067.28 KB |        0.04 |
