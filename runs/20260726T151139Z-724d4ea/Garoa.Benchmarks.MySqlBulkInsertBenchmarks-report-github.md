```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2
  Job-LYONEC : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **20.013 ms** |  **1.1887 ms** |  **0.7074 ms** |  **1.00** |    **0.05** |   **2794.9 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  10.702 ms |  2.1885 ms |  1.3024 ms |  0.54 |    0.06 |   477.17 KB |        0.17 |
| GaroaBulk      | 1000  |   9.001 ms |  0.5187 ms |  0.3087 ms |  0.45 |    0.02 |   111.45 KB |        0.04 |
|                |       |            |            |            |       |         |             |             |
| **Dapper**         | **10000** | **132.428 ms** | **20.9310 ms** | **12.4557 ms** |  **1.01** |    **0.12** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 |  88.364 ms | 24.2478 ms | 16.0384 ms |  0.67 |    0.13 |   5260.7 KB |        0.19 |
| GaroaBulk      | 10000 |  75.264 ms | 17.8702 ms | 11.8200 ms |  0.57 |    0.10 |  1060.66 KB |        0.04 |
