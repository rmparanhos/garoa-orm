
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-XIYCCU : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method      | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
------------ |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
 **Dapper**      | **1000**  |  **16.93 ms** |  **0.463 ms** |  **0.275 ms** |  **1.00** |    **0.02** |  **2656.59 KB** |       **1.000** |
 StagingCopy | 1000  |  12.79 ms |  1.314 ms |  0.782 ms |  0.76 |    0.05 |     5.47 KB |       0.002 |
             |       |           |           |           |       |         |             |             |
 **Dapper**      | **10000** | **126.99 ms** | **19.646 ms** | **12.995 ms** |  **1.01** |    **0.13** | **26554.64 KB** |       **1.000** |
 StagingCopy | 10000 |  64.84 ms | 25.951 ms | 17.165 ms |  0.52 |    0.14 |     6.45 KB |       0.000 |
