```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-ERJJUF : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**        | **1000**  |  **16.72 ms** |  **0.280 ms** |  **0.146 ms** |  **1.00** |    **0.01** |  **2656.88 KB** |       **1.000** |
| ManualStaging | 1000  |  12.47 ms |  0.720 ms |  0.429 ms |  0.75 |    0.03 |     5.14 KB |       0.002 |
| GaroaBulk     | 1000  |  11.98 ms |  0.333 ms |  0.174 ms |  0.72 |    0.01 |     9.27 KB |       0.003 |
|               |       |           |           |           |       |         |             |             |
| **Dapper**        | **10000** | **121.95 ms** | **13.768 ms** |  **9.107 ms** |  **1.00** |    **0.10** | **26554.03 KB** |       **1.000** |
| ManualStaging | 10000 |  63.00 ms | 27.216 ms | 18.002 ms |  0.52 |    0.15 |     5.52 KB |       0.000 |
| GaroaBulk     | 10000 |  63.49 ms | 17.253 ms | 10.267 ms |  0.52 |    0.09 |     10.3 KB |       0.000 |
