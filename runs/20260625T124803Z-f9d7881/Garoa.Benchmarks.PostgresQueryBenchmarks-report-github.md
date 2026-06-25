```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-LZGQSS : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  IterationCount=8  LaunchCount=1  
WarmupCount=3  

```
| Method         | Rows | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|--------------- |----- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
| **Dapper**         | **1**    | **149.6 μs** |  **7.40 μs** |  **3.87 μs** |  **1.00** |    **0.03** |     **968 B** |        **1.00** |
| Manual         | 1    | 148.9 μs |  6.75 μs |  3.53 μs |  1.00 |    0.03 |     616 B |        0.64 |
| Garoa          | 1    | 147.1 μs |  6.51 μs |  2.89 μs |  0.98 |    0.03 |     680 B |        0.70 |
| GaroaGenerated | 1    | 153.7 μs |  4.00 μs |  2.09 μs |  1.03 |    0.03 |     680 B |        0.70 |
|                |      |          |          |          |       |         |           |             |
| **Dapper**         | **100**  | **235.0 μs** | **10.41 μs** |  **5.44 μs** |  **1.00** |    **0.03** |   **24057 B** |        **1.00** |
| Manual         | 100  | 244.7 μs |  8.10 μs |  3.59 μs |  1.04 |    0.03 |   16577 B |        0.69 |
| Garoa          | 100  | 247.0 μs |  8.96 μs |  4.69 μs |  1.05 |    0.03 |   16641 B |        0.69 |
| GaroaGenerated | 100  | 247.2 μs |  6.93 μs |  3.62 μs |  1.05 |    0.03 |   16641 B |        0.69 |
|                |      |          |          |          |       |         |           |             |
| **Dapper**         | **1000** | **753.4 μs** | **11.07 μs** |  **5.79 μs** |  **1.00** |    **0.01** |  **229492 B** |        **1.00** |
| Manual         | 1000 | 861.9 μs | 28.51 μs | 14.91 μs |  1.14 |    0.02 |  157003 B |        0.68 |
| Garoa          | 1000 | 874.5 μs | 24.98 μs | 13.06 μs |  1.16 |    0.02 |  157066 B |        0.68 |
| GaroaGenerated | 1000 | 874.1 μs | 10.21 μs |  5.34 μs |  1.16 |    0.01 |  157069 B |        0.68 |
