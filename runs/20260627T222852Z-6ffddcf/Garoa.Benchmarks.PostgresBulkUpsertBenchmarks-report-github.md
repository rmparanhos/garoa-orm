```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-VSUAXN : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**        | **1000**  |  **17.68 ms** |  **0.589 ms** |  **0.308 ms** |  **1.00** |    **0.02** |  **2656.93 KB** |       **1.000** |
| ManualStaging | 1000  |  12.96 ms |  0.663 ms |  0.347 ms |  0.73 |    0.02 |     4.53 KB |       0.002 |
| GaroaBulk     | 1000  |  13.03 ms |  1.578 ms |  1.043 ms |  0.74 |    0.06 |     9.55 KB |       0.004 |
|               |       |           |           |           |       |         |             |             |
| **Dapper**        | **10000** | **132.33 ms** | **21.620 ms** | **14.301 ms** |  **1.01** |    **0.15** | **26554.64 KB** |       **1.000** |
| ManualStaging | 10000 |  59.88 ms | 20.551 ms | 12.230 ms |  0.46 |    0.10 |     6.08 KB |       0.000 |
| GaroaBulk     | 10000 |  59.39 ms | 22.438 ms | 13.352 ms |  0.45 |    0.11 |    10.91 KB |       0.000 |
