```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2
  Job-LYONEC : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**        | **1000**  |  **16.64 ms** |  **3.384 ms** |  **2.014 ms** |  **1.01** |    **0.16** |  **2656.55 KB** |       **1.000** |
| ManualStaging | 1000  |  11.52 ms |  1.467 ms |  0.970 ms |  0.70 |    0.09 |     5.14 KB |       0.002 |
| GaroaBulk     | 1000  |  10.71 ms |  0.239 ms |  0.142 ms |  0.65 |    0.07 |     9.45 KB |       0.004 |
|               |       |           |           |           |       |         |             |             |
| **Dapper**        | **10000** | **151.97 ms** | **72.222 ms** | **47.770 ms** |  **1.08** |    **0.43** | **26554.59 KB** |       **1.000** |
| ManualStaging | 10000 |  53.12 ms | 11.652 ms |  6.934 ms |  0.38 |    0.11 |     6.83 KB |       0.000 |
| GaroaBulk     | 10000 |  58.65 ms | 23.718 ms | 15.688 ms |  0.42 |    0.15 |    10.44 KB |       0.000 |
