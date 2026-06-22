
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-SDEXJD : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method      | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
------------ |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
 **NaiveInsert** | **1000**  |   **9.985 ms** |  **0.4885 ms** |  **0.2555 ms** |  **1.00** |    **0.03** |  **1844.79 KB** |        **1.00** |
 GaroaBulk   | 1000  |   6.743 ms |  0.3229 ms |  0.1921 ms |  0.68 |    0.02 |    73.27 KB |        0.04 |
             |       |            |            |            |       |         |             |             |
 **NaiveInsert** | **10000** | **113.719 ms** | **25.7792 ms** | **17.0513 ms** |  **1.02** |    **0.22** | **18438.98 KB** |        **1.00** |
 GaroaBulk   | 10000 |  37.665 ms | 13.9174 ms |  7.2790 ms |  0.34 |    0.08 |   708.62 KB |        0.04 |
