```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-FEVIDF : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **14.063 ms** |  **1.7113 ms** |  **0.8951 ms** |  **1.00** |    **0.09** |  **2655.48 KB** |       **1.000** |
| ManualCopy | 1000  |   5.656 ms |  0.1967 ms |  0.1171 ms |  0.40 |    0.03 |     2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   6.061 ms |  0.3531 ms |  0.2101 ms |  0.43 |    0.03 |     3.24 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **131.390 ms** | **58.1204 ms** | **38.4431 ms** |  **1.07** |    **0.40** | **26544.08 KB** |       **1.000** |
| ManualCopy | 10000 |  40.330 ms | 20.9518 ms | 13.8583 ms |  0.33 |    0.14 |     3.43 KB |       0.000 |
| GaroaBulk  | 10000 |  44.274 ms | 13.6938 ms |  9.0576 ms |  0.36 |    0.11 |     4.55 KB |       0.000 |
