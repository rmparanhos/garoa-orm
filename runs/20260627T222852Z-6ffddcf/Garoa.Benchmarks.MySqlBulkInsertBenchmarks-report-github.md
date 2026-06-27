```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-VSUAXN : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **31.34 ms** | **12.494 ms** |  **8.264 ms** |  **1.07** |    **0.39** |   **2794.9 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  15.80 ms |  6.526 ms |  3.884 ms |  0.54 |    0.19 |   477.17 KB |        0.17 |
| GaroaBulk      | 1000  |  13.96 ms |  4.285 ms |  2.550 ms |  0.47 |    0.15 |   111.56 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **172.97 ms** | **26.045 ms** | **17.227 ms** |  **1.01** |    **0.13** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 |  99.44 ms | 15.037 ms |  8.948 ms |  0.58 |    0.07 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  84.32 ms |  9.437 ms |  4.936 ms |  0.49 |    0.05 |  1060.78 KB |        0.04 |
