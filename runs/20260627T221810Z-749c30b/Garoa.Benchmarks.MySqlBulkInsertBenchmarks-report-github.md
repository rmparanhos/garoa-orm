```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-WUFBPK : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **29.80 ms** | **10.160 ms** |  **6.720 ms** |  **1.05** |    **0.32** |   **2794.9 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  14.06 ms |  1.835 ms |  1.214 ms |  0.49 |    0.11 |   477.17 KB |        0.17 |
| GaroaBulk      | 1000  |  12.00 ms |  1.831 ms |  1.211 ms |  0.42 |    0.10 |   111.56 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **157.55 ms** | **22.307 ms** | **14.755 ms** |  **1.01** |    **0.12** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 | 111.34 ms | 30.013 ms | 19.852 ms |  0.71 |    0.14 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  99.11 ms | 16.977 ms | 11.229 ms |  0.63 |    0.09 |  1060.78 KB |        0.04 |
