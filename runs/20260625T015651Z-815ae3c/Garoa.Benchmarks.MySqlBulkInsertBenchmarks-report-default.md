
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-WOTHUO : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
 **Dapper**         | **1000**  |  **23.92 ms** |  **4.103 ms** |  **2.442 ms** |  **1.01** |    **0.13** |   **2794.9 KB** |        **1.00** |
 ManualBulkCopy | 1000  |  10.92 ms |  0.842 ms |  0.440 ms |  0.46 |    0.04 |   477.17 KB |        0.17 |
 GaroaBulk      | 1000  |  10.19 ms |  0.718 ms |  0.427 ms |  0.43 |    0.04 |   111.56 KB |        0.04 |
                |       |           |           |           |       |         |             |             |
 **Dapper**         | **10000** | **139.21 ms** | **14.551 ms** |  **8.659 ms** |  **1.00** |    **0.08** | **27936.61 KB** |        **1.00** |
 ManualBulkCopy | 10000 | 103.80 ms | 22.492 ms | 14.877 ms |  0.75 |    0.11 |  5260.66 KB |        0.19 |
 GaroaBulk      | 10000 |  85.33 ms | 15.661 ms | 10.359 ms |  0.62 |    0.08 |  1060.78 KB |        0.04 |
