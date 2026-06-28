```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-MQVEKV : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **15.962 ms** |  **3.6099 ms** |  **2.1482 ms** |  **1.01** |    **0.17** |  **2655.24 KB** |       **1.000** |
| ManualCopy | 1000  |   6.759 ms |  0.3147 ms |  0.1873 ms |  0.43 |    0.05 |     2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   6.679 ms |  0.1877 ms |  0.1117 ms |  0.42 |    0.05 |     3.57 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **163.606 ms** | **61.3007 ms** | **40.5466 ms** |  **1.07** |    **0.41** | **26543.52 KB** |       **1.000** |
| ManualCopy | 10000 |  43.221 ms | 30.8007 ms | 20.3728 ms |  0.28 |    0.16 |     3.66 KB |       0.000 |
| GaroaBulk  | 10000 |  42.355 ms | 24.8376 ms | 14.7805 ms |  0.28 |    0.12 |     4.88 KB |       0.000 |
