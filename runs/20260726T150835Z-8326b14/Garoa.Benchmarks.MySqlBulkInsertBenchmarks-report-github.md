```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon 6973P-C, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-FDCKSM : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev     | Median    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|-----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **22.91 ms** |  **12.92 ms** |   **7.689 ms** |  **24.13 ms** |  **1.11** |    **0.51** |  **2912.09 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  23.56 ms |  39.59 ms |  23.557 ms |  10.20 ms |  1.14 |    1.19 |   477.17 KB |        0.16 |
| GaroaBulk      | 1000  |  27.59 ms |  36.40 ms |  21.663 ms |  21.72 ms |  1.33 |    1.12 |   111.45 KB |        0.04 |
|                |       |           |           |            |           |       |         |             |             |
| **Dapper**         | **10000** | **424.51 ms** | **596.22 ms** | **394.362 ms** | **173.81 ms** |  **1.88** |    **2.23** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 | 115.25 ms |  58.61 ms |  30.653 ms | 104.64 ms |  0.51 |    0.34 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 | 633.47 ms | 551.41 ms | 364.721 ms | 553.26 ms |  2.81 |    2.44 |  1060.66 KB |        0.04 |
