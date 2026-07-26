```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2
  Job-LYONEC : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **14.272 ms** |  **2.9806 ms** |  **1.5589 ms** |  **1.01** |    **0.14** |  **2655.52 KB** |       **1.000** |
| ManualCopy | 1000  |   5.814 ms |  0.3466 ms |  0.2062 ms |  0.41 |    0.04 |     2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   6.278 ms |  1.0016 ms |  0.5960 ms |  0.44 |    0.06 |     3.42 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **140.852 ms** | **47.9987 ms** | **31.7482 ms** |  **1.05** |    **0.34** | **26543.98 KB** |       **1.000** |
| ManualCopy | 10000 |  42.367 ms | 19.2333 ms | 12.7216 ms |  0.32 |    0.12 |     4.04 KB |       0.000 |
| GaroaBulk  | 10000 |  42.850 ms | 12.1052 ms |  8.0069 ms |  0.32 |    0.09 |     4.73 KB |       0.000 |
