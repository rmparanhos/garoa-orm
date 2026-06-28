```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-ERJCVT : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **12.880 ms** |  **1.5116 ms** |  **0.7906 ms** |  **1.00** |    **0.08** |  **2655.15 KB** |       **1.000** |
| ManualCopy | 1000  |   5.501 ms |  0.5015 ms |  0.2984 ms |  0.43 |    0.03 |      2.4 KB |       0.001 |
| GaroaBulk  | 1000  |   5.192 ms |  0.0984 ms |  0.0514 ms |  0.40 |    0.02 |     3.24 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **135.136 ms** | **59.3025 ms** | **39.2249 ms** |  **1.08** |    **0.42** | **26542.58 KB** |       **1.000** |
| ManualCopy | 10000 |  40.051 ms | 15.7357 ms | 10.4082 ms |  0.32 |    0.12 |     4.04 KB |       0.000 |
| GaroaBulk  | 10000 |  41.589 ms | 17.0349 ms | 11.2675 ms |  0.33 |    0.12 |     4.88 KB |       0.000 |
