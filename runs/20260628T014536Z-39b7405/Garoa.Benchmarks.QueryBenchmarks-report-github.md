```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-TPXHZZ : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  IterationCount=8  LaunchCount=1  
WarmupCount=3  

```
| Method         | Rows | Mean         | Error      | StdDev    | Ratio | Allocated | Alloc Ratio |
|--------------- |----- |-------------:|-----------:|----------:|------:|----------:|------------:|
| **Dapper**         | **1**    |     **8.384 μs** |  **0.0319 μs** | **0.0167 μs** |  **1.00** |    **1608 B** |        **1.00** |
| Manual         | 1    |     6.678 μs |  0.0769 μs | 0.0402 μs |  0.80 |     880 B |        0.55 |
| Garoa          | 1    |     7.707 μs |  0.0874 μs | 0.0388 μs |  0.92 |    1200 B |        0.75 |
| GaroaGenerated | 1    |     7.370 μs |  0.0630 μs | 0.0280 μs |  0.88 |    1200 B |        0.75 |
|                |      |              |            |           |       |           |             |
| **Dapper**         | **100**  |   **144.571 μs** |  **1.4954 μs** | **0.6640 μs** |  **1.00** |   **24696 B** |        **1.00** |
| Manual         | 100  |   135.081 μs |  2.2409 μs | 0.9950 μs |  0.93 |   16840 B |        0.68 |
| Garoa          | 100  |   159.395 μs |  0.9829 μs | 0.4364 μs |  1.10 |   17160 B |        0.69 |
| GaroaGenerated | 100  |   138.504 μs |  1.4338 μs | 0.6366 μs |  0.96 |   17160 B |        0.69 |
|                |      |              |            |           |       |           |             |
| **Dapper**         | **1000** | **1,364.843 μs** | **16.9227 μs** | **8.8509 μs** |  **1.00** |  **229905 B** |        **1.00** |
| Manual         | 1000 | 1,274.178 μs |  9.5216 μs | 4.2276 μs |  0.93 |  157249 B |        0.68 |
| Garoa          | 1000 | 1,799.154 μs | 14.8862 μs | 7.7858 μs |  1.32 |  157569 B |        0.69 |
| GaroaGenerated | 1000 | 1,292.870 μs |  9.4416 μs | 4.1921 μs |  0.95 |  157569 B |        0.69 |
