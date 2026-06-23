
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V45, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-OQMTER : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method     | Rows  | Mean       | Error      | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
----------- |------ |-----------:|-----------:|----------:|------:|--------:|------------:|------------:|
 **Dapper**     | **1000**  |   **8.226 ms** |   **6.094 ms** |  **3.627 ms** |  **1.15** |    **0.63** |  **2655.29 KB** |       **1.000** |
 ManualCopy | 1000  |   4.367 ms |   2.253 ms |  1.178 ms |  0.61 |    0.26 |     2.73 KB |       0.001 |
 GaroaBulk  | 1000  |   6.299 ms |   3.149 ms |  1.874 ms |  0.88 |    0.39 |    73.88 KB |       0.028 |
            |       |            |            |           |       |         |             |             |
 **Dapper**     | **10000** | **157.417 ms** | **135.561 ms** | **89.665 ms** |  **1.47** |    **1.35** | **26543.52 KB** |       **1.000** |
 ManualCopy | 10000 |  28.024 ms |  26.636 ms | 15.851 ms |  0.26 |    0.24 |     4.32 KB |       0.000 |
 GaroaBulk  | 10000 |  31.195 ms |  33.109 ms | 17.317 ms |  0.29 |    0.26 |    707.4 KB |       0.027 |
