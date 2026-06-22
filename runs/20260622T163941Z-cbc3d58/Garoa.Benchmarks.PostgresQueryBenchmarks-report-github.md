```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-HBIJXU : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=8  LaunchCount=1  
WarmupCount=3  

```
| Method | Rows | Mean     | Error   | StdDev  | Ratio | Gen0   | Allocated | Alloc Ratio |
|------- |----- |---------:|--------:|--------:|------:|-------:|----------:|------------:|
| **Dapper** | **1**    | **201.2 μs** | **2.23 μs** | **0.99 μs** |  **1.00** |      **-** |     **969 B** |        **1.00** |
| Garoa  | 1    | 199.6 μs | 1.65 μs | 0.73 μs |  0.99 |      - |     681 B |        0.70 |
|        |      |          |         |         |       |        |           |             |
| **Dapper** | **100**  | **278.6 μs** | **3.82 μs** | **1.70 μs** |  **1.00** |      **-** |   **24057 B** |        **1.00** |
| Garoa  | 100  | 302.8 μs | 3.58 μs | 1.87 μs |  1.09 |      - |   16641 B |        0.69 |
|        |      |          |         |         |       |        |           |             |
| **Dapper** | **1000** | **799.9 μs** | **9.68 μs** | **4.30 μs** |  **1.00** | **1.9531** |  **229376 B** |        **1.00** |
| Garoa  | 1000 | 971.2 μs | 9.58 μs | 5.01 μs |  1.21 |      - |  157066 B |        0.68 |
