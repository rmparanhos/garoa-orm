```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-WUFBPK : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**        | **1000**  |  **18.62 ms** |  **1.821 ms** |  **1.083 ms** |  **1.00** |    **0.08** |  **2656.59 KB** |       **1.000** |
| ManualStaging | 1000  |  13.16 ms |  3.042 ms |  1.810 ms |  0.71 |    0.10 |     5.14 KB |       0.002 |
| GaroaBulk     | 1000  |  11.91 ms |  0.577 ms |  0.343 ms |  0.64 |    0.04 |     9.27 KB |       0.003 |
|               |       |           |           |           |       |         |             |             |
| **Dapper**        | **10000** | **133.67 ms** | **34.542 ms** | **20.555 ms** |  **1.02** |    **0.20** | **26554.64 KB** |       **1.000** |
| ManualStaging | 10000 |  55.35 ms |  8.166 ms |  4.271 ms |  0.42 |    0.07 |     6.45 KB |       0.000 |
| GaroaBulk     | 10000 |  66.79 ms | 32.468 ms | 21.476 ms |  0.51 |    0.17 |    10.59 KB |       0.000 |
