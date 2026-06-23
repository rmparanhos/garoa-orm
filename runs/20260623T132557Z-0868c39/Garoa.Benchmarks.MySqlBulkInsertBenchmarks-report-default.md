
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V45, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-MSPUSS : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method         | Rows  | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Allocated   | Alloc Ratio |
--------------- |------ |-----------:|----------:|----------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**         | **1000**  |  **19.201 ms** | **11.617 ms** |  **7.684 ms** |  **15.951 ms** |  **1.12** |    **0.56** |   **2794.9 KB** |        **1.00** |
 ManualBulkCopy | 1000  |   9.498 ms |  3.767 ms |  1.970 ms |   9.651 ms |  0.56 |    0.20 |   477.17 KB |        0.17 |
 GaroaBulk      | 1000  |  11.402 ms |  8.632 ms |  5.137 ms |   8.561 ms |  0.67 |    0.36 |   111.53 KB |        0.04 |
                |       |            |           |           |            |       |         |             |             |
 **Dapper**         | **10000** | **154.915 ms** | **44.234 ms** | **26.323 ms** | **142.902 ms** |  **1.02** |    **0.22** | **27936.61 KB** |        **1.00** |
 ManualBulkCopy | 10000 |  98.085 ms | 26.583 ms | 13.904 ms |  93.910 ms |  0.65 |    0.12 |  5260.66 KB |        0.19 |
 GaroaBulk      | 10000 |  94.017 ms | 24.343 ms | 16.101 ms |  92.855 ms |  0.62 |    0.13 |  1060.75 KB |        0.04 |
