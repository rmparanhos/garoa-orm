
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-FZVXNG : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=8  LaunchCount=1  
WarmupCount=3  

 Method | Rows | Mean         | Error      | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
------- |----- |-------------:|-----------:|----------:|------:|-------:|----------:|------------:|
 **Dapper** | **1**    |     **8.396 μs** |  **0.1260 μs** | **0.0559 μs** |  **1.00** | **0.0153** |   **1.57 KB** |        **1.00** |
 Garoa  | 1    |     7.549 μs |  0.0529 μs | 0.0235 μs |  0.90 |      - |   1.17 KB |        0.75 |
        |      |              |            |           |       |        |           |             |
 **Dapper** | **100**  |   **128.026 μs** |  **1.7832 μs** | **0.7917 μs** |  **1.00** | **0.2441** |  **24.12 KB** |        **1.00** |
 Garoa  | 100  |   136.926 μs |  1.7319 μs | 0.9058 μs |  1.07 |      - |  16.76 KB |        0.69 |
        |      |              |            |           |       |        |           |             |
 **Dapper** | **1000** | **1,204.813 μs** | **13.2783 μs** | **5.8956 μs** |  **1.00** | **1.9531** | **224.52 KB** |        **1.00** |
 Garoa  | 1000 | 1,304.835 μs |  4.6403 μs | 2.0603 μs |  1.08 |      - | 153.88 KB |        0.69 |
