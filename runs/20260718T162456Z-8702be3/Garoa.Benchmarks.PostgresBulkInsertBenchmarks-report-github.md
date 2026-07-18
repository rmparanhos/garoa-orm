```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2
  Job-CXNUMO : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **13.842 ms** |  **1.9961 ms** |  **1.0440 ms** |  **1.00** |    **0.10** |  **2655.57 KB** |       **1.000** |
| ManualCopy | 1000  |   5.645 ms |  0.0731 ms |  0.0435 ms |  0.41 |    0.03 |     2.68 KB |       0.001 |
| GaroaBulk  | 1000  |   5.825 ms |  0.3928 ms |  0.2338 ms |  0.42 |    0.03 |     3.42 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **134.312 ms** | **56.4597 ms** | **37.3446 ms** |  **1.07** |    **0.40** | **26542.91 KB** |       **1.000** |
| ManualCopy | 10000 |  40.641 ms | 21.8769 ms | 14.4702 ms |  0.32 |    0.14 |     3.43 KB |       0.000 |
| GaroaBulk  | 10000 |  44.467 ms | 13.2518 ms |  8.7652 ms |  0.35 |    0.11 |     4.08 KB |       0.000 |
