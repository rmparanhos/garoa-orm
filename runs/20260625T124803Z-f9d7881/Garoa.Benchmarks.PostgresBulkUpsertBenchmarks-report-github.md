```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-GNBIVL : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method      | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|------------ |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**      | **1000**  |  **16.27 ms** |  **1.781 ms** |  **0.931 ms** |  **1.00** |    **0.08** |   **2657.2 KB** |       **1.000** |
| StagingCopy | 1000  |  11.31 ms |  1.257 ms |  0.831 ms |  0.70 |    0.06 |     4.53 KB |       0.002 |
|             |       |           |           |           |       |         |             |             |
| **Dapper**      | **10000** | **150.80 ms** | **62.820 ms** | **41.552 ms** |  **1.07** |    **0.39** | **26554.64 KB** |       **1.000** |
| StagingCopy | 10000 |  69.34 ms | 29.315 ms | 19.390 ms |  0.49 |    0.18 |     6.78 KB |       0.000 |
