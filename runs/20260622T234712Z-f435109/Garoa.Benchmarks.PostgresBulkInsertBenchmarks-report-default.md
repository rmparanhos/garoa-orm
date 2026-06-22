
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-GNNNVU : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method    | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
---------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**    | **1000**  |  **13.162 ms** |  **0.7792 ms** |  **0.4075 ms** |  **1.00** |    **0.04** |  **2655.52 KB** |        **1.00** |
 GaroaBulk | 1000  |   6.350 ms |  0.1309 ms |  0.0779 ms |  0.48 |    0.01 |    73.93 KB |        0.03 |
           |       |            |            |            |       |         |             |             |
 **Dapper**    | **10000** | **135.480 ms** | **53.0778 ms** | **35.1077 ms** |  **1.06** |    **0.37** | **26544.36 KB** |        **1.00** |
 GaroaBulk | 10000 |  37.106 ms | 12.9978 ms |  6.7981 ms |  0.29 |    0.09 |   707.96 KB |        0.03 |
