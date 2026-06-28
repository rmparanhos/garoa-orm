```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-DMGYAW : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**        | **1000**  |  **18.38 ms** |  **2.527 ms** |  **1.504 ms** |  **1.01** |    **0.11** |   **2655.7 KB** |       **1.000** |
| ManualStaging | 1000  |  13.33 ms |  2.068 ms |  1.368 ms |  0.73 |    0.09 |     6.08 KB |       0.002 |
| GaroaBulk     | 1000  |  11.89 ms |  0.389 ms |  0.204 ms |  0.65 |    0.05 |      8.9 KB |       0.003 |
|               |       |           |           |           |       |         |             |             |
| **Dapper**        | **10000** | **158.50 ms** | **61.578 ms** | **40.730 ms** |  **1.06** |    **0.36** | **26556.98 KB** |       **1.000** |
| ManualStaging | 10000 |  63.19 ms | 26.798 ms | 17.725 ms |  0.42 |    0.15 |     6.45 KB |       0.000 |
| GaroaBulk     | 10000 |  59.31 ms | 19.611 ms | 11.670 ms |  0.40 |    0.12 |    10.59 KB |       0.000 |
