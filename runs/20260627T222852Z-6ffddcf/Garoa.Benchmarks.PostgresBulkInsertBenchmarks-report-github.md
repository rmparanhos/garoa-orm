```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-VSUAXN : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **15.988 ms** |  **1.9566 ms** |  **1.0233 ms** |  **1.00** |    **0.08** |  **2655.52 KB** |       **1.000** |
| ManualCopy | 1000  |   6.846 ms |  0.3064 ms |  0.1824 ms |  0.43 |    0.03 |     1.79 KB |       0.001 |
| GaroaBulk  | 1000  |   6.759 ms |  0.0760 ms |  0.0398 ms |  0.42 |    0.02 |     3.57 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **139.137 ms** | **70.9839 ms** | **46.9515 ms** |  **1.10** |    **0.49** | **26543.52 KB** |       **1.000** |
| ManualCopy | 10000 |  40.908 ms | 20.7042 ms | 12.3207 ms |  0.32 |    0.14 |      4.6 KB |       0.000 |
| GaroaBulk  | 10000 |  45.689 ms | 32.6982 ms | 21.6278 ms |  0.36 |    0.20 |     5.16 KB |       0.000 |
