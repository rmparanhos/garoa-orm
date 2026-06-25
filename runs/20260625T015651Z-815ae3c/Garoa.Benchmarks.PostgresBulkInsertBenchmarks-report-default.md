
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-WOTHUO : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**     | **1000**  |  **15.506 ms** |  **1.5713 ms** |  **0.8218 ms** |  **1.00** |    **0.07** |   **2655.2 KB** |       **1.000** |
 ManualCopy | 1000  |   6.691 ms |  0.0858 ms |  0.0511 ms |  0.43 |    0.02 |     2.73 KB |       0.001 |
 GaroaBulk  | 1000  |   6.591 ms |  0.0477 ms |  0.0284 ms |  0.43 |    0.02 |     3.24 KB |       0.001 |
            |       |            |            |            |       |         |             |             |
 **Dapper**     | **10000** | **147.415 ms** | **65.6287 ms** | **43.4093 ms** |  **1.09** |    **0.45** | **26544.92 KB** |       **1.000** |
 ManualCopy | 10000 |  42.430 ms | 20.2139 ms | 10.5722 ms |  0.31 |    0.12 |     4.04 KB |       0.000 |
 GaroaBulk  | 10000 |  40.415 ms | 21.7343 ms | 11.3674 ms |  0.30 |    0.12 |     4.51 KB |       0.000 |
