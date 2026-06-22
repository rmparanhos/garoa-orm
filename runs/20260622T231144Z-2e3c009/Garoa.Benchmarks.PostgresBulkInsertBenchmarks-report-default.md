
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-RGWGPC : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method      | Rows  | Mean         | Error       | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
------------ |------ |-------------:|------------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**      | **1000**  |   **456.470 ms** |  **22.6629 ms** | **14.9901 ms** |  **1.00** |    **0.04** |  **1508.09 KB** |        **1.00** |
 NaiveInsert | 1000  |    11.938 ms |   2.1503 ms |  1.1246 ms |  0.03 |    0.00 |  1844.79 KB |        1.22 |
 GaroaBulk   | 1000  |     7.581 ms |   0.4836 ms |  0.2529 ms |  0.02 |    0.00 |    73.88 KB |        0.05 |
             |       |              |             |            |       |         |             |             |
 **Dapper**      | **10000** | **4,542.459 ms** | **143.6481 ms** | **95.0143 ms** |  **1.00** |    **0.03** | **15078.73 KB** |        **1.00** |
 NaiveInsert | 10000 |   133.921 ms |  27.0349 ms | 17.8819 ms |  0.03 |    0.00 | 18439.45 KB |        1.22 |
 GaroaBulk   | 10000 |    45.892 ms |  33.2411 ms | 21.9869 ms |  0.01 |    0.00 |   708.01 KB |        0.05 |
