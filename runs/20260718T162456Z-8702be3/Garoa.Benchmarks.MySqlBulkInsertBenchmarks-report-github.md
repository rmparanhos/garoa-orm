```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2
  Job-CXNUMO : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **29.078 ms** | **10.3291 ms** |  **6.8321 ms** |  **1.06** |    **0.35** |  **2912.09 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  11.312 ms |  2.2784 ms |  1.5070 ms |  0.41 |    0.11 |   477.17 KB |        0.16 |
| GaroaBulk      | 1000  |   9.404 ms |  0.4211 ms |  0.2785 ms |  0.34 |    0.08 |   111.45 KB |        0.04 |
|                |       |            |            |            |       |         |             |             |
| **Dapper**         | **10000** | **134.067 ms** | **13.0015 ms** |  **7.7370 ms** |  **1.00** |    **0.08** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 |  91.034 ms | 24.3635 ms | 14.4983 ms |  0.68 |    0.11 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  76.264 ms | 16.1768 ms | 10.6999 ms |  0.57 |    0.08 |  1060.66 KB |        0.04 |
