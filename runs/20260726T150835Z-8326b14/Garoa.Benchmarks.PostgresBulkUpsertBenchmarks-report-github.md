```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon 6973P-C, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-FDCKSM : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean       | Error       | StdDev      | Median     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|-------------- |------ |-----------:|------------:|------------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**        | **1000**  |  **16.359 ms** |   **3.2952 ms** |   **2.1796 ms** |  **15.353 ms** |  **1.01** |    **0.18** |  **2656.59 KB** |       **1.000** |
| ManualStaging | 1000  |   8.260 ms |   1.5103 ms |   0.8988 ms |   8.448 ms |  0.51 |    0.08 |     5.05 KB |       0.002 |
| GaroaBulk     | 1000  |   8.108 ms |   0.9542 ms |   0.5678 ms |   7.982 ms |  0.50 |    0.07 |     9.41 KB |       0.004 |
|               |       |            |             |             |            |       |         |             |             |
| **Dapper**        | **10000** | **193.252 ms** | **159.9169 ms** | **105.7751 ms** | **224.453 ms** |  **1.39** |    **1.17** | **26554.97 KB** |       **1.000** |
| ManualStaging | 10000 |  72.444 ms |  90.9464 ms |  54.1208 ms |  54.946 ms |  0.52 |    0.52 |     7.34 KB |       0.000 |
| GaroaBulk     | 10000 | 109.332 ms | 129.1529 ms |  85.4266 ms |  63.947 ms |  0.79 |    0.82 |    11.38 KB |       0.000 |
