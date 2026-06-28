```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-ZONUUP : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated  | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|-----------:|------------:|
| **Dapper**        | **1000**  |  **16.99 ms** |  **0.585 ms** |  **0.348 ms** |  **1.00** |    **0.03** | **2656.59 KB** |       **1.000** |
| ManualStaging | 1000  |  12.39 ms |  0.802 ms |  0.478 ms |  0.73 |    0.03 |    5.14 KB |       0.002 |
| GaroaBulk     | 1000  |  13.30 ms |  1.896 ms |  1.254 ms |  0.78 |    0.07 |    9.27 KB |       0.003 |
|               |       |           |           |           |       |         |            |             |
| **Dapper**        | **10000** | **121.56 ms** | **21.691 ms** | **12.908 ms** |  **1.01** |    **0.14** | **26553.7 KB** |       **1.000** |
| ManualStaging | 10000 |  55.57 ms | 13.176 ms |  6.891 ms |  0.46 |    0.07 |    7.06 KB |       0.000 |
| GaroaBulk     | 10000 |  60.77 ms | 20.910 ms | 12.443 ms |  0.50 |    0.11 |    9.65 KB |       0.000 |
