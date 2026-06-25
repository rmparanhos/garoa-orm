```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-DNKTBB : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **27.82 ms** |  **9.742 ms** |  **6.444 ms** |  **1.04** |    **0.31** |  **2912.09 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  11.82 ms |  1.641 ms |  0.977 ms |  0.44 |    0.09 |   477.17 KB |        0.16 |
| GaroaBulk      | 1000  |  10.44 ms |  0.258 ms |  0.154 ms |  0.39 |    0.08 |   111.56 KB |        0.04 |
|                |       |           |           |           |       |         |             |             |
| **Dapper**         | **10000** | **155.61 ms** | **22.084 ms** | **14.607 ms** |  **1.01** |    **0.12** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 |  98.92 ms | 22.764 ms | 15.057 ms |  0.64 |    0.11 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  81.03 ms | 12.193 ms |  8.065 ms |  0.52 |    0.07 |  1060.78 KB |        0.04 |
