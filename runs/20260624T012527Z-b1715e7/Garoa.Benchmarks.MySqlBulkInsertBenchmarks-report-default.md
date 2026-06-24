
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-UVLIRA : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method         | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
--------------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**         | **1000**  |  **20.238 ms** |  **1.8010 ms** |  **1.0717 ms** |  **1.00** |    **0.07** |  **2912.09 KB** |        **1.00** |
 ManualBulkCopy | 1000  |  10.555 ms |  2.0132 ms |  1.1980 ms |  0.52 |    0.06 |   477.17 KB |        0.16 |
 GaroaBulk      | 1000  |   8.841 ms |  0.2751 ms |  0.1439 ms |  0.44 |    0.02 |   111.56 KB |        0.04 |
                |       |            |            |            |       |         |             |             |
 **Dapper**         | **10000** | **148.922 ms** | **33.1351 ms** | **21.9168 ms** |  **1.02** |    **0.19** | **27936.61 KB** |        **1.00** |
 ManualBulkCopy | 10000 |  89.750 ms | 26.2716 ms | 17.3771 ms |  0.61 |    0.14 |  5260.66 KB |        0.19 |
 GaroaBulk      | 10000 |  71.892 ms | 12.1725 ms |  8.0514 ms |  0.49 |    0.08 |  1060.78 KB |        0.04 |
