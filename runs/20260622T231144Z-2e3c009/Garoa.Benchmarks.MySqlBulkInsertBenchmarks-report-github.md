```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-RGWGPC : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method      | Rows  | Mean         | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|------------ |------ |-------------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**      | **1000**  |  **1,306.15 ms** | **262.592 ms** | **156.264 ms** |  **1.01** |    **0.17** |  **1065.15 KB** |        **1.00** |
| NaiveInsert | 1000  |     23.28 ms |   7.796 ms |   4.639 ms |  0.02 |    0.00 |  2148.32 KB |        2.02 |
| GaroaBulk   | 1000  |     13.16 ms |   2.099 ms |   1.388 ms |  0.01 |    0.00 |   111.53 KB |        0.10 |
|             |       |              |            |            |       |         |             |             |
| **Dapper**      | **10000** | **14,805.02 ms** | **757.673 ms** | **501.154 ms** | **1.001** |    **0.05** | **10627.65 KB** |        **1.00** |
| NaiveInsert | 10000 |    143.44 ms |  12.871 ms |   7.660 ms | 0.010 |    0.00 | 21470.83 KB |        2.02 |
| GaroaBulk   | 10000 |     88.40 ms |  13.777 ms |   8.199 ms | 0.006 |    0.00 |  1060.75 KB |        0.10 |
