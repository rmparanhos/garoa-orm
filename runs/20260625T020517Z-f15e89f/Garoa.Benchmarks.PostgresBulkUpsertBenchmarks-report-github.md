```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-DNKTBB : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method      | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|------------ |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**      | **1000**  |  **18.84 ms** |  **2.023 ms** |  **1.204 ms** |  **1.00** |    **0.08** |  **2656.22 KB** |       **1.000** |
| StagingCopy | 1000  |  14.90 ms |  2.751 ms |  1.820 ms |  0.79 |    0.10 |     4.25 KB |       0.002 |
|             |       |           |           |           |       |         |             |             |
| **Dapper**      | **10000** | **160.20 ms** | **77.614 ms** | **51.337 ms** |  **1.08** |    **0.43** | **26554.64 KB** |       **1.000** |
| StagingCopy | 10000 |  64.60 ms | 29.956 ms | 19.814 ms |  0.44 |    0.17 |     7.02 KB |       0.000 |
