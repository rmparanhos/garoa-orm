```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-WOTHUO : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method      | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|------------ |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**      | **1000**  |  **16.72 ms** |  **0.362 ms** |  **0.215 ms** |  **1.00** |    **0.02** |  **2656.59 KB** |       **1.000** |
| StagingCopy | 1000  |  12.90 ms |  2.288 ms |  1.362 ms |  0.77 |    0.08 |     5.14 KB |       0.002 |
|             |       |           |           |           |       |         |             |             |
| **Dapper**      | **10000** | **120.76 ms** | **17.604 ms** | **10.476 ms** |  **1.01** |    **0.11** | **26555.58 KB** |       **1.000** |
| StagingCopy | 10000 |  61.70 ms | 21.478 ms | 12.781 ms |  0.51 |    0.11 |     5.84 KB |       0.000 |
