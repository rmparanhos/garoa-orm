```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-OMXNJL : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **26.59 ms** |  **8.577 ms** |  **5.104 ms** |  **1.03** |    **0.26** |   **2794.9 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  14.24 ms |  4.778 ms |  2.843 ms |  0.55 |    0.14 |   477.17 KB |        0.17 |
| GaroaBulk      | 1000  |  10.56 ms |  0.557 ms |  0.291 ms |  0.41 |    0.07 |   111.56 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **150.90 ms** | **17.095 ms** | **10.173 ms** |  **1.00** |    **0.09** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 |  98.25 ms |  6.003 ms |  3.140 ms |  0.65 |    0.05 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  88.67 ms | 14.013 ms |  8.339 ms |  0.59 |    0.06 |  1060.78 KB |        0.04 |
