```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-DMGYAW : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **14.467 ms** |  **0.5231 ms** |  **0.2736 ms** |  **1.00** |    **0.03** |  **2655.52 KB** |       **1.000** |
| ManualCopy | 1000  |   6.651 ms |  0.1778 ms |  0.1058 ms |  0.46 |    0.01 |     2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   6.745 ms |  0.4096 ms |  0.2709 ms |  0.47 |    0.02 |     3.24 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **166.801 ms** | **82.0465 ms** | **54.2687 ms** |  **1.12** |    **0.54** | **26542.58 KB** |       **1.000** |
| ManualCopy | 10000 |  41.447 ms | 17.5809 ms |  9.1951 ms |  0.28 |    0.12 |     4.04 KB |       0.000 |
| GaroaBulk  | 10000 |  46.293 ms | 26.5331 ms | 17.5500 ms |  0.31 |    0.16 |     4.88 KB |       0.000 |
