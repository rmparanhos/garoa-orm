```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-OMXNJL : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **16.160 ms** |  **1.4832 ms** |  **0.7758 ms** |  **1.00** |    **0.06** |  **2656.13 KB** |       **1.000** |
| ManualCopy | 1000  |   6.727 ms |  0.1330 ms |  0.0792 ms |  0.42 |    0.02 |     2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   6.833 ms |  0.3310 ms |  0.1731 ms |  0.42 |    0.02 |     3.24 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **115.958 ms** | **32.8761 ms** | **17.1949 ms** |  **1.02** |    **0.20** | **26542.58 KB** |       **1.000** |
| ManualCopy | 10000 |  46.428 ms | 27.2356 ms | 18.0147 ms |  0.41 |    0.16 |     4.04 KB |       0.000 |
| GaroaBulk  | 10000 |  46.762 ms | 29.7818 ms | 19.6988 ms |  0.41 |    0.18 |     4.55 KB |       0.000 |
