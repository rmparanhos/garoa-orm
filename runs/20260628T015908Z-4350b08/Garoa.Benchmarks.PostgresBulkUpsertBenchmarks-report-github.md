```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-MQVEKV : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**        | **1000**  |  **17.17 ms** |  **1.028 ms** |  **0.611 ms** |  **1.00** |    **0.05** |  **2655.66 KB** |       **1.000** |
| ManualStaging | 1000  |  12.83 ms |  1.839 ms |  1.217 ms |  0.75 |    0.07 |     5.14 KB |       0.002 |
| GaroaBulk     | 1000  |  13.20 ms |  1.806 ms |  1.195 ms |  0.77 |    0.07 |     8.95 KB |       0.003 |
|               |       |           |           |           |       |         |             |             |
| **Dapper**        | **10000** | **134.69 ms** | **18.411 ms** | **10.956 ms** |  **1.01** |    **0.11** | **26554.64 KB** |       **1.000** |
| ManualStaging | 10000 |  55.65 ms |  9.172 ms |  4.797 ms |  0.42 |    0.05 |     5.84 KB |       0.000 |
| GaroaBulk     | 10000 |  56.44 ms | 10.322 ms |  5.399 ms |  0.42 |    0.05 |    11.52 KB |       0.000 |
