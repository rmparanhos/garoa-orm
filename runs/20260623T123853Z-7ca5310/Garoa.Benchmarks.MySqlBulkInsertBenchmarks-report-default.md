
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V45, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-OQMTER : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method         | Rows  | Mean      | Error     | StdDev     | Median     | Ratio | RatioSD | Allocated   | Alloc Ratio |
--------------- |------ |----------:|----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**         | **1000**  |  **33.48 ms** |  **31.44 ms** |  **20.794 ms** |  **26.890 ms** |  **1.39** |    **1.19** |   **2794.9 KB** |        **1.00** |
 ManualBulkCopy | 1000  |  24.15 ms |  18.66 ms |  12.343 ms |  25.084 ms |  1.01 |    0.77 |   477.17 KB |        0.17 |
 GaroaBulk      | 1000  |  10.76 ms |  11.66 ms |   6.939 ms |   7.266 ms |  0.45 |    0.39 |   111.53 KB |        0.04 |
                |       |           |           |            |            |       |         |             |             |
 **Dapper**         | **10000** | **262.65 ms** | **193.02 ms** | **127.672 ms** | **218.026 ms** |  **1.22** |    **0.80** | **27936.61 KB** |        **1.00** |
 ManualBulkCopy | 10000 | 140.59 ms |  53.15 ms |  31.630 ms | 131.901 ms |  0.65 |    0.32 |  5260.66 KB |        0.19 |
 GaroaBulk      | 10000 | 201.91 ms | 171.89 ms | 102.290 ms | 169.114 ms |  0.93 |    0.63 |  1060.75 KB |        0.04 |
