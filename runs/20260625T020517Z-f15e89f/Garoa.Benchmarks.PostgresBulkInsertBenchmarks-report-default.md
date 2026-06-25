
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-DNKTBB : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**     | **1000**  |  **18.504 ms** |  **5.7695 ms** |  **3.8162 ms** |  **1.03** |    **0.27** |  **2772.71 KB** |       **1.000** |
 ManualCopy | 1000  |   7.348 ms |  0.4040 ms |  0.2404 ms |  0.41 |    0.07 |      2.4 KB |       0.001 |
 GaroaBulk  | 1000  |   7.043 ms |  0.1187 ms |  0.0621 ms |  0.39 |    0.07 |     4.18 KB |       0.002 |
            |       |            |            |            |       |         |             |             |
 **Dapper**     | **10000** | **149.210 ms** | **75.5613 ms** | **49.9791 ms** |  **1.11** |    **0.50** | **26543.52 KB** |       **1.000** |
 ManualCopy | 10000 |  40.702 ms | 24.4701 ms | 14.5618 ms |  0.30 |    0.14 |     3.38 KB |       0.000 |
 GaroaBulk  | 10000 |  41.008 ms | 30.0421 ms | 19.8710 ms |  0.30 |    0.17 |     4.88 KB |       0.000 |
