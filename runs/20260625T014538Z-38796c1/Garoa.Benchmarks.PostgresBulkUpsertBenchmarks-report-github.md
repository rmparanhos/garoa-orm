```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-IUIATW : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method      | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated  | Alloc Ratio |
|------------ |------ |----------:|----------:|----------:|------:|--------:|-----------:|------------:|
| **Dapper**      | **1000**  |  **16.64 ms** |  **2.489 ms** |  **1.302 ms** |  **1.01** |    **0.10** | **2656.59 KB** |       **1.000** |
| StagingCopy | 1000  |  11.60 ms |  2.100 ms |  1.249 ms |  0.70 |    0.09 |    4.81 KB |       0.002 |
|             |       |           |           |           |       |         |            |             |
| **Dapper**      | **10000** | **125.83 ms** | **40.216 ms** | **23.932 ms** |  **1.03** |    **0.24** | **26553.7 KB** |       **1.000** |
| StagingCopy | 10000 |  59.41 ms | 18.538 ms | 12.262 ms |  0.48 |    0.12 |    6.13 KB |       0.000 |
