```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-BBFTZT : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **19.062 ms** |  **5.9797 ms** |  **3.9552 ms** |  **1.04** |    **0.29** |  **2656.13 KB** |       **1.000** |
| ManualCopy | 1000  |   6.762 ms |  0.3127 ms |  0.1861 ms |  0.37 |    0.07 |     3.05 KB |       0.001 |
| GaroaBulk  | 1000  |   6.965 ms |  0.8706 ms |  0.5758 ms |  0.38 |    0.08 |     3.57 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **121.785 ms** | **34.3321 ms** | **17.9563 ms** |  **1.02** |    **0.19** | **26543.52 KB** |       **1.000** |
| ManualCopy | 10000 |  40.909 ms | 19.2086 ms | 10.0465 ms |  0.34 |    0.09 |     3.99 KB |       0.000 |
| GaroaBulk  | 10000 |  42.643 ms | 24.0215 ms | 14.2948 ms |  0.36 |    0.12 |     5.82 KB |       0.000 |
