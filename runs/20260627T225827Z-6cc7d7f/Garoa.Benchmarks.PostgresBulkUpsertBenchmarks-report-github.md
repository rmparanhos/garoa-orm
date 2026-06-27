```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-FEVIDF : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**        | **1000**  |  **15.76 ms** |  **0.534 ms** |  **0.318 ms** |  **1.00** |    **0.03** |  **2656.55 KB** |       **1.000** |
| ManualStaging | 1000  |  11.81 ms |  2.017 ms |  1.200 ms |  0.75 |    0.07 |     5.14 KB |       0.002 |
| GaroaBulk     | 1000  |  11.02 ms |  0.361 ms |  0.215 ms |  0.70 |    0.02 |     9.27 KB |       0.003 |
|               |       |           |           |           |       |         |             |             |
| **Dapper**        | **10000** | **159.03 ms** | **50.296 ms** | **33.268 ms** |  **1.04** |    **0.29** | **26556.05 KB** |       **1.000** |
| ManualStaging | 10000 |  56.12 ms | 13.967 ms |  7.305 ms |  0.37 |    0.08 |     5.52 KB |       0.000 |
| GaroaBulk     | 10000 |  64.00 ms | 24.661 ms | 16.312 ms |  0.42 |    0.13 |    10.54 KB |       0.000 |
