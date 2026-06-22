
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-HUGUFK : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method    | Rows  | Mean      | Error     | StdDev   | Ratio | RatioSD | Allocated   | Alloc Ratio |
---------- |------ |----------:|----------:|---------:|------:|--------:|------------:|------------:|
 **Dapper**    | **1000**  |  **27.13 ms** |  **1.887 ms** | **1.123 ms** |  **1.00** |    **0.05** |   **2794.9 KB** |        **1.00** |
 GaroaBulk | 1000  |  10.57 ms |  0.650 ms | 0.430 ms |  0.39 |    0.02 |   111.53 KB |        0.04 |
           |       |           |           |          |       |         |             |             |
 **Dapper**    | **10000** | **148.91 ms** | **14.195 ms** | **8.447 ms** |  **1.00** |    **0.08** | **27936.61 KB** |        **1.00** |
 GaroaBulk | 10000 |  85.29 ms | 10.924 ms | 7.226 ms |  0.57 |    0.06 |  1060.75 KB |        0.04 |
