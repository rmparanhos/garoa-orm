
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-SYXJIQ : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method      | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
------------ |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
 **Dapper**      | **1000**  |  **17.38 ms** |  **0.388 ms** |  **0.231 ms** |  **1.00** |    **0.02** |  **2656.59 KB** |       **1.000** |
 StagingCopy | 1000  |  13.59 ms |  2.061 ms |  1.363 ms |  0.78 |    0.08 |     5.42 KB |       0.002 |
             |       |           |           |           |       |         |             |             |
 **Dapper**      | **10000** | **140.65 ms** | **45.171 ms** | **29.878 ms** |  **1.04** |    **0.29** | **26555.58 KB** |       **1.000** |
 StagingCopy | 10000 |  57.96 ms | 13.267 ms |  6.939 ms |  0.43 |    0.09 |     6.45 KB |       0.000 |
