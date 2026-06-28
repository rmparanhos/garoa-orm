```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-ZONUUP : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **14.763 ms** |  **0.5222 ms** |  **0.2731 ms** |  **1.00** |    **0.02** |  **2655.48 KB** |       **1.000** |
| ManualCopy | 1000  |   6.666 ms |  0.1416 ms |  0.0843 ms |  0.45 |    0.01 |     2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   6.693 ms |  0.2595 ms |  0.1544 ms |  0.45 |    0.01 |     3.57 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **113.202 ms** | **30.0948 ms** | **15.7401 ms** |  **1.02** |    **0.19** | **26543.52 KB** |       **1.000** |
| ManualCopy | 10000 |  40.454 ms | 18.3779 ms |  9.6120 ms |  0.36 |    0.10 |     4.04 KB |       0.000 |
| GaroaBulk  | 10000 |  41.813 ms | 20.0447 ms | 10.4837 ms |  0.38 |    0.10 |     5.21 KB |       0.000 |
