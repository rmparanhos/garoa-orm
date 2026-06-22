
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-HBIJXU : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=8  LaunchCount=1  
WarmupCount=3  

 Method | Rows | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
------- |----- |-----------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 **Dapper** | **1**    |   **222.1 μs** |  **7.44 μs** |  **3.89 μs** |  **1.00** |    **0.02** |      **-** |   **2.54 KB** |        **1.00** |
 Garoa  | 1    |   217.7 μs | 12.54 μs |  6.56 μs |  0.98 |    0.03 |      - |   2.19 KB |        0.86 |
        |      |            |          |          |       |         |        |           |             |
 **Dapper** | **100**  |   **330.3 μs** |  **2.88 μs** |  **1.03 μs** |  **1.00** |    **0.00** |      **-** |  **25.09 KB** |        **1.00** |
 Garoa  | 100  |   332.9 μs |  4.63 μs |  2.05 μs |  1.01 |    0.01 |      - |  24.74 KB |        0.99 |
        |      |            |          |          |       |         |        |           |             |
 **Dapper** | **1000** | **1,072.2 μs** | **33.85 μs** | **15.03 μs** |  **1.00** |    **0.02** | **1.9531** | **225.49 KB** |        **1.00** |
 Garoa  | 1000 | 1,070.5 μs | 18.06 μs |  8.02 μs |  1.00 |    0.01 | 1.9531 | 225.14 KB |        1.00 |
