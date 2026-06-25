```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V45, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-NVVPSZ : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method      | Rows  | Mean       | Error      | StdDev     | Median     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|------------ |------ |-----------:|-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**      | **1000**  |   **8.603 ms** |  **0.5098 ms** |  **0.2666 ms** |   **8.489 ms** |  **1.00** |    **0.04** |  **2656.59 KB** |       **1.000** |
| StagingCopy | 1000  |   8.322 ms |  6.0112 ms |  3.5772 ms |   6.596 ms |  0.97 |    0.40 |     5.14 KB |       0.002 |
|             |       |            |            |            |            |       |         |             |             |
| **Dapper**      | **10000** | **128.119 ms** | **65.6900 ms** | **43.4499 ms** | **108.106 ms** |  **1.09** |    **0.46** | **26556.52 KB** |       **1.000** |
| StagingCopy | 10000 |  77.503 ms | 96.7382 ms | 63.9863 ms |  33.145 ms |  0.66 |    0.56 |     6.13 KB |       0.000 |
