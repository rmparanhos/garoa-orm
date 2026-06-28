```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-TPXHZZ : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  IterationCount=8  LaunchCount=1  
WarmupCount=3  

```
| Method         | Rows | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|--------------- |----- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
| **Dapper**         | **1**    | **152.3 μs** |  **2.28 μs** |  **1.01 μs** |  **1.00** |    **0.01** |     **968 B** |        **1.00** |
| Manual         | 1    | 149.8 μs |  4.30 μs |  2.25 μs |  0.98 |    0.02 |     616 B |        0.64 |
| Garoa          | 1    | 155.5 μs |  3.03 μs |  1.59 μs |  1.02 |    0.01 |     680 B |        0.70 |
| GaroaGenerated | 1    | 151.1 μs |  3.68 μs |  1.63 μs |  0.99 |    0.01 |     680 B |        0.70 |
|                |      |          |          |          |       |         |           |             |
| **Dapper**         | **100**  | **236.4 μs** | **12.77 μs** |  **6.68 μs** |  **1.00** |    **0.04** |   **24057 B** |        **1.00** |
| Manual         | 100  | 246.8 μs |  7.13 μs |  3.73 μs |  1.04 |    0.03 |   16577 B |        0.69 |
| Garoa          | 100  | 247.5 μs |  5.52 μs |  2.89 μs |  1.05 |    0.03 |   16641 B |        0.69 |
| GaroaGenerated | 100  | 253.7 μs | 11.32 μs |  5.92 μs |  1.07 |    0.04 |   16641 B |        0.69 |
|                |      |          |          |          |       |         |           |             |
| **Dapper**         | **1000** | **788.8 μs** |  **7.18 μs** |  **3.19 μs** |  **1.00** |    **0.01** |  **229336 B** |        **1.00** |
| Manual         | 1000 | 876.4 μs | 28.90 μs | 12.83 μs |  1.11 |    0.02 |  156997 B |        0.68 |
| Garoa          | 1000 | 904.9 μs | 23.83 μs | 12.46 μs |  1.15 |    0.02 |  157073 B |        0.68 |
| GaroaGenerated | 1000 | 896.2 μs | 21.98 μs | 11.49 μs |  1.14 |    0.01 |  157070 B |        0.68 |
