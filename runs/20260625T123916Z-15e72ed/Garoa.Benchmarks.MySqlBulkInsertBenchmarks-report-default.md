
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-XIYCCU : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
 **Dapper**         | **1000**  |  **28.37 ms** | **10.005 ms** |  **6.618 ms** |  **1.04** |    **0.31** |   **2794.9 KB** |        **1.00** |
 ManualBulkCopy | 1000  |  12.40 ms |  2.214 ms |  1.318 ms |  0.46 |    0.10 |   477.17 KB |        0.17 |
 GaroaBulk      | 1000  |  10.63 ms |  0.659 ms |  0.392 ms |  0.39 |    0.08 |   111.56 KB |        0.04 |
                |       |           |           |           |       |         |             |             |
 **Dapper**         | **10000** | **145.52 ms** |  **9.571 ms** |  **5.006 ms** |  **1.00** |    **0.05** | **27936.61 KB** |        **1.00** |
 ManualBulkCopy | 10000 | 105.93 ms | 22.037 ms | 14.576 ms |  0.73 |    0.10 |  5260.66 KB |        0.19 |
 GaroaBulk      | 10000 |  88.24 ms | 14.763 ms |  9.765 ms |  0.61 |    0.07 |  1060.78 KB |        0.04 |
