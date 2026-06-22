```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-GNNNVU : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method    | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|---------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**    | **1000**  |  **26.340 ms** |  **9.1148 ms** |  **6.0289 ms** |  **1.05** |    **0.32** |  **2912.09 KB** |        **1.00** |
| GaroaBulk | 1000  |   9.322 ms |  0.7723 ms |  0.5108 ms |  0.37 |    0.08 |   111.53 KB |        0.04 |
|           |       |            |            |            |       |         |             |             |
| **Dapper**    | **10000** | **145.702 ms** | **47.3595 ms** | **31.3254 ms** |  **1.04** |    **0.28** | **27936.61 KB** |        **1.00** |
| GaroaBulk | 10000 |  75.817 ms | 15.0567 ms |  9.9591 ms |  0.54 |    0.12 |  1060.75 KB |        0.04 |
