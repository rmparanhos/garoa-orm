```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-ERJCVT : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method         | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|--------------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**         | **1000**  |  **22.302 ms** |  **3.5044 ms** |  **2.0854 ms** |  **1.01** |    **0.13** |   **2794.9 KB** |        **1.00** |
| ManualBulkCopy | 1000  |  11.511 ms |  1.9362 ms |  1.1522 ms |  0.52 |    0.07 |   477.17 KB |        0.17 |
| GaroaBulk      | 1000  |   9.562 ms |  0.6974 ms |  0.4613 ms |  0.43 |    0.04 |   111.56 KB |        0.04 |
|                |       |            |            |            |       |         |             |             |
| **Dapper**         | **10000** | **146.896 ms** | **38.7801 ms** | **23.0774 ms** |  **1.02** |    **0.20** | **27936.61 KB** |        **1.00** |
| ManualBulkCopy | 10000 |  94.914 ms | 21.5101 ms | 14.2276 ms |  0.66 |    0.13 |  5260.66 KB |        0.19 |
| GaroaBulk      | 10000 |  78.249 ms | 16.4316 ms | 10.8685 ms |  0.54 |    0.10 |  1060.78 KB |        0.04 |
