```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2
  Job-TVLTUZ : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**        | **1000**  |  **17.37 ms** |  **0.410 ms** |  **0.244 ms** |  **1.00** |    **0.02** |  **2656.22 KB** |       **1.000** |
| ManualStaging | 1000  |  14.53 ms |  3.055 ms |  2.021 ms |  0.84 |    0.11 |     6.08 KB |       0.002 |
| GaroaBulk     | 1000  |  12.46 ms |  0.508 ms |  0.266 ms |  0.72 |    0.02 |     9.41 KB |       0.004 |
|               |       |           |           |           |       |         |             |             |
| **Dapper**        | **10000** | **122.15 ms** |  **9.699 ms** |  **5.073 ms** |  **1.00** |    **0.05** | **26554.92 KB** |       **1.000** |
| ManualStaging | 10000 |  59.73 ms | 20.689 ms | 12.311 ms |  0.49 |    0.10 |     6.45 KB |       0.000 |
| GaroaBulk     | 10000 |  60.97 ms | 20.749 ms | 12.347 ms |  0.50 |    0.10 |    11.38 KB |       0.000 |
