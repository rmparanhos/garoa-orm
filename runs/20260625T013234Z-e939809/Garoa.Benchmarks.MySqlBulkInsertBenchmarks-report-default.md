
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V45, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-NVVPSZ : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method         | Rows  | Mean      | Error     | StdDev    | Median     | Ratio | RatioSD | Allocated   | Alloc Ratio |
--------------- |------ |----------:|----------:|----------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**         | **1000**  |  **27.10 ms** |  **41.43 ms** |  **21.67 ms** |  **20.496 ms** |  **1.36** |    **1.26** |   **2794.9 KB** |        **1.00** |
 ManualBulkCopy | 1000  |  16.52 ms |  39.59 ms |  20.71 ms |   7.471 ms |  0.83 |    1.12 |   477.17 KB |        0.17 |
 GaroaBulk      | 1000  |  35.11 ms |  48.21 ms |  31.88 ms |  18.303 ms |  1.76 |    1.82 |   111.56 KB |        0.04 |
                |       |           |           |           |            |       |         |             |             |
 **Dapper**         | **10000** | **622.26 ms** | **681.32 ms** | **405.44 ms** | **465.695 ms** |  **1.53** |    **1.48** | **27936.61 KB** |        **1.00** |
 ManualBulkCopy | 10000 | 242.33 ms | 206.14 ms | 136.35 ms | 233.849 ms |  0.60 |    0.54 |  5260.66 KB |        0.19 |
 GaroaBulk      | 10000 | 298.53 ms | 321.51 ms | 212.66 ms | 243.893 ms |  0.73 |    0.75 |  1060.78 KB |        0.04 |
