```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-FEVIDF : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **26.072 ms** |  **9.6301 ms** |  **6.3697 ms** |  **1.05** |    **0.34** |  **2912.09 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  10.830 ms |  1.3409 ms |  0.7013 ms |  0.44 |    0.10 |   477.17 KB |        0.16 |
| GaroaBulk      | 1000  |   9.449 ms |  0.6358 ms |  0.4205 ms |  0.38 |    0.08 |   111.56 KB |        0.04 |
|                |       |            |            |            |       |         |             |             |
| **Dapper**         | **10000** | **133.091 ms** | **17.3553 ms** | **10.3279 ms** |  **1.01** |    **0.10** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 |  86.073 ms | 17.3360 ms | 11.4667 ms |  0.65 |    0.10 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  77.021 ms | 15.2088 ms | 10.0596 ms |  0.58 |    0.08 |  1060.78 KB |        0.04 |
