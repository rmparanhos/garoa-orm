```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon 6973P-C, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-FDCKSM : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error      | StdDev     | Median     | Ratio | RatioSD | Allocated  | Alloc Ratio |
|-------------- |------ |----------:|-----------:|-----------:|-----------:|------:|--------:|-----------:|------------:|
| **Dapper**        | **1000**  |  **97.71 ms** | **145.744 ms** |  **96.400 ms** |  **53.842 ms** |  **4.23** |    **7.43** | **2795.67 KB** |        **1.00** |
| ManualStaging | 1000  |  76.28 ms | 180.366 ms | 107.333 ms |  11.243 ms |  3.30 |    7.40 |  478.86 KB |        0.17 |
| GaroaBulk     | 1000  |  10.01 ms |   2.328 ms |   1.540 ms |   9.902 ms |  0.43 |    0.48 |  118.27 KB |        0.04 |
|               |       |           |            |            |            |       |         |            |             |
| **Dapper**        | **10000** | **197.57 ms** | **171.418 ms** | **102.008 ms** | **136.307 ms** |  **1.21** |    **0.79** | **27944.3 KB** |        **1.00** |
| ManualStaging | 10000 | 274.68 ms | 227.523 ms | 150.492 ms | 270.232 ms |  1.69 |    1.15 | 5262.36 KB |        0.19 |
| GaroaBulk     | 10000 | 188.45 ms | 141.116 ms |  83.976 ms | 209.964 ms |  1.16 |    0.69 | 1067.49 KB |        0.04 |
