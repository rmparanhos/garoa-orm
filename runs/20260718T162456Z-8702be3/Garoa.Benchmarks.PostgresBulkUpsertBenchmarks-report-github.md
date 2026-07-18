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
| **Dapper**        | **1000**  |  **15.59 ms** |  **0.292 ms** |  **0.174 ms** |  **1.00** |    **0.01** | **2656.03 KB** |       **1.000** |
| ManualStaging | 1000  |  11.94 ms |  2.154 ms |  1.425 ms |  0.77 |    0.09 |     4.2 KB |       0.002 |
| GaroaBulk     | 1000  |  12.28 ms |  2.159 ms |  1.428 ms |  0.79 |    0.09 |    9.17 KB |       0.003 |
|               |       |           |           |           |       |         |            |             |
| **Dapper**        | **10000** | **134.83 ms** | **30.680 ms** | **18.257 ms** |  **1.02** |    **0.18** | **26553.7 KB** |       **1.000** |
| ManualStaging | 10000 |  65.03 ms | 24.469 ms | 16.184 ms |  0.49 |    0.13 |    6.13 KB |       0.000 |
| GaroaBulk     | 10000 |  64.19 ms | 29.185 ms | 19.304 ms |  0.48 |    0.15 |   10.72 KB |       0.000 |
