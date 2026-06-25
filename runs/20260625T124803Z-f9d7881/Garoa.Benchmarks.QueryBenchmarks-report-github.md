```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-LZGQSS : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  IterationCount=8  LaunchCount=1  
WarmupCount=3  

```
| Method         | Rows | Mean         | Error      | StdDev    | Ratio | Allocated | Alloc Ratio |
|--------------- |----- |-------------:|-----------:|----------:|------:|----------:|------------:|
| **Dapper**         | **1**    |     **8.118 μs** |  **0.0561 μs** | **0.0249 μs** |  **1.00** |    **1608 B** |        **1.00** |
| Manual         | 1    |     6.498 μs |  0.0168 μs | 0.0075 μs |  0.80 |     880 B |        0.55 |
| Garoa          | 1    |     7.746 μs |  0.0268 μs | 0.0140 μs |  0.95 |    1200 B |        0.75 |
| GaroaGenerated | 1    |     7.484 μs |  0.1202 μs | 0.0534 μs |  0.92 |    1200 B |        0.75 |
|                |      |              |            |           |       |           |             |
| **Dapper**         | **100**  |   **147.986 μs** |  **2.0389 μs** | **1.0664 μs** |  **1.00** |   **24696 B** |        **1.00** |
| Manual         | 100  |   133.285 μs |  0.9713 μs | 0.3464 μs |  0.90 |   16840 B |        0.68 |
| Garoa          | 100  |   154.744 μs |  0.6325 μs | 0.2808 μs |  1.05 |   17160 B |        0.69 |
| GaroaGenerated | 100  |   135.137 μs |  0.4748 μs | 0.2483 μs |  0.91 |   17160 B |        0.69 |
|                |      |              |            |           |       |           |             |
| **Dapper**         | **1000** | **1,345.437 μs** | **11.6264 μs** | **5.1622 μs** |  **1.00** |  **229905 B** |        **1.00** |
| Manual         | 1000 | 1,270.746 μs |  4.8943 μs | 2.1731 μs |  0.94 |  157249 B |        0.68 |
| Garoa          | 1000 | 1,514.673 μs |  6.9986 μs | 2.4958 μs |  1.13 |  157569 B |        0.69 |
| GaroaGenerated | 1000 | 1,277.054 μs |  2.9102 μs | 1.5221 μs |  0.95 |  157569 B |        0.69 |
