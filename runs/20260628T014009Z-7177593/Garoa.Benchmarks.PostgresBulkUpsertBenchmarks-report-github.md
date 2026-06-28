```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-OMXNJL : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**        | **1000**  |  **18.46 ms** |  **2.404 ms** |  **1.430 ms** |  **1.01** |    **0.10** |  **2656.59 KB** |       **1.000** |
| ManualStaging | 1000  |  13.89 ms |  2.466 ms |  1.467 ms |  0.76 |    0.09 |     5.14 KB |       0.002 |
| GaroaBulk     | 1000  |  13.49 ms |  2.480 ms |  1.641 ms |  0.73 |    0.10 |     8.34 KB |       0.003 |
|               |       |           |           |           |       |         |             |             |
| **Dapper**        | **10000** | **167.54 ms** | **66.876 ms** | **44.234 ms** |  **1.07** |    **0.38** | **26554.64 KB** |       **1.000** |
| ManualStaging | 10000 |  55.94 ms | 13.010 ms |  6.804 ms |  0.36 |    0.10 |     7.06 KB |       0.000 |
| GaroaBulk     | 10000 |  59.52 ms | 22.298 ms | 13.269 ms |  0.38 |    0.13 |    10.02 KB |       0.000 |
