```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-SDEXJD : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method      | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|------------ |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **NaiveInsert** | **1000**  |  **17.778 ms** |  **0.4586 ms** |  **0.2729 ms** |  **1.00** |    **0.02** |  **2148.32 KB** |        **1.00** |
| GaroaBulk   | 1000  |   9.027 ms |  0.7335 ms |  0.3837 ms |  0.51 |    0.02 |   111.53 KB |        0.05 |
|             |       |            |            |            |       |         |             |             |
| **NaiveInsert** | **10000** | **135.959 ms** | **35.6523 ms** | **23.5818 ms** |  **1.02** |    **0.23** | **21470.83 KB** |        **1.00** |
| GaroaBulk   | 10000 |  79.975 ms | 15.6315 ms | 10.3393 ms |  0.60 |    0.12 |  1060.75 KB |        0.05 |
