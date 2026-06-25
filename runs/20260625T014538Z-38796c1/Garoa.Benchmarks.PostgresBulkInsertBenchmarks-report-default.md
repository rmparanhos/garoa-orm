
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-IUIATW : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**     | **1000**  |  **12.937 ms** |  **0.4008 ms** |  **0.2096 ms** |  **1.00** |    **0.02** |  **2655.52 KB** |       **1.000** |
 ManualCopy | 1000  |   5.603 ms |  0.1874 ms |  0.0980 ms |  0.43 |    0.01 |     2.73 KB |       0.001 |
 GaroaBulk  | 1000  |   6.087 ms |  0.6399 ms |  0.4233 ms |  0.47 |    0.03 |     3.57 KB |       0.001 |
            |       |            |            |            |       |         |             |             |
 **Dapper**     | **10000** | **140.418 ms** | **45.8847 ms** | **30.3499 ms** |  **1.05** |    **0.34** | **26543.52 KB** |       **1.000** |
 ManualCopy | 10000 |  41.340 ms | 17.4519 ms | 11.5433 ms |  0.31 |    0.11 |     4.65 KB |       0.000 |
 GaroaBulk  | 10000 |  40.896 ms | 21.6103 ms | 14.2939 ms |  0.31 |    0.13 |     5.49 KB |       0.000 |
