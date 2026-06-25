```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-SYXJIQ : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **15.722 ms** |  **1.6421 ms** |  **0.8588 ms** |  **1.00** |    **0.07** |  **2655.85 KB** |       **1.000** |
| ManualCopy | 1000  |   6.739 ms |  0.2407 ms |  0.1259 ms |  0.43 |    0.02 |     2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   6.651 ms |  0.0834 ms |  0.0436 ms |  0.42 |    0.02 |     3.29 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **123.163 ms** | **35.5592 ms** | **21.1607 ms** |  **1.03** |    **0.24** | **26542.58 KB** |       **1.000** |
| ManualCopy | 10000 |  40.735 ms | 21.7310 ms | 11.3657 ms |  0.34 |    0.11 |     4.04 KB |       0.000 |
| GaroaBulk  | 10000 |  46.887 ms | 26.7037 ms | 17.6629 ms |  0.39 |    0.16 |     5.49 KB |       0.000 |
