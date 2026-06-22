
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-UNFOOJ : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=8  LaunchCount=1  
WarmupCount=3  

 Method         | Rows | Mean       | Error    | StdDev  | Ratio | Gen0   | Allocated | Alloc Ratio |
--------------- |----- |-----------:|---------:|--------:|------:|-------:|----------:|------------:|
 **Dapper**         | **1**    |   **200.9 μs** |  **2.46 μs** | **1.28 μs** |  **1.00** |      **-** |     **969 B** |        **1.00** |
 Garoa          | 1    |   198.8 μs |  1.75 μs | 0.91 μs |  0.99 |      - |     681 B |        0.70 |
 GaroaGenerated | 1    |   198.7 μs |  4.17 μs | 2.18 μs |  0.99 |      - |     681 B |        0.70 |
                |      |            |          |         |       |        |           |             |
 **Dapper**         | **100**  |   **285.5 μs** |  **3.70 μs** | **1.94 μs** |  **1.00** |      **-** |   **24057 B** |        **1.00** |
 Garoa          | 100  |   302.1 μs |  6.25 μs | 3.27 μs |  1.06 |      - |   16641 B |        0.69 |
 GaroaGenerated | 100  |   295.7 μs |  4.51 μs | 2.36 μs |  1.04 |      - |   16641 B |        0.69 |
                |      |            |          |         |       |        |           |             |
 **Dapper**         | **1000** |   **810.1 μs** | **14.78 μs** | **7.73 μs** |  **1.00** | **1.9531** |  **229412 B** |        **1.00** |
 Garoa          | 1000 | 1,034.6 μs | 11.67 μs | 5.18 μs |  1.28 |      - |  157063 B |        0.68 |
 GaroaGenerated | 1000 |   943.1 μs | 10.36 μs | 5.42 μs |  1.16 |      - |  157089 B |        0.68 |
