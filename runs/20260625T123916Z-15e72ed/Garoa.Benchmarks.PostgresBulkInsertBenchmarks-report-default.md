
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-XIYCCU : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**     | **1000**  |  **16.602 ms** |  **3.9434 ms** |  **2.3467 ms** |  **1.02** |    **0.18** |  **2654.91 KB** |       **1.000** |
 ManualCopy | 1000  |   6.809 ms |  0.2904 ms |  0.1728 ms |  0.42 |    0.05 |     2.73 KB |       0.001 |
 GaroaBulk  | 1000  |   6.775 ms |  0.3113 ms |  0.1852 ms |  0.41 |    0.05 |     3.57 KB |       0.001 |
            |       |            |            |            |       |         |             |             |
 **Dapper**     | **10000** | **158.733 ms** | **59.6035 ms** | **39.4241 ms** |  **1.06** |    **0.38** | **26543.52 KB** |       **1.000** |
 ManualCopy | 10000 |  40.653 ms | 17.6375 ms |  9.2248 ms |  0.27 |    0.09 |     3.43 KB |       0.000 |
 GaroaBulk  | 10000 |  45.726 ms | 33.7213 ms | 22.3045 ms |  0.31 |    0.17 |     4.51 KB |       0.000 |
