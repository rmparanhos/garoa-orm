```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-ERJJUF : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **15.013 ms** |  **0.1856 ms** |  **0.0971 ms** |  **1.00** |    **0.01** |  **2655.52 KB** |       **1.000** |
| ManualCopy | 1000  |   6.777 ms |  0.0700 ms |  0.0417 ms |  0.45 |    0.00 |     2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   6.767 ms |  0.2943 ms |  0.1539 ms |  0.45 |    0.01 |     3.57 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **157.002 ms** | **70.9589 ms** | **46.9349 ms** |  **1.09** |    **0.45** | **26544.41 KB** |       **1.000** |
| ManualCopy | 10000 |  43.188 ms | 24.3117 ms | 14.4675 ms |  0.30 |    0.13 |      3.1 KB |       0.000 |
| GaroaBulk  | 10000 |  42.891 ms | 23.9342 ms | 14.2428 ms |  0.30 |    0.13 |     5.16 KB |       0.000 |
