
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-GNBIVL : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method         | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
--------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
 **Dapper**         | **1000**  |  **22.45 ms** |  **1.957 ms** |  **1.165 ms** |  **1.00** |    **0.07** |   **2794.9 KB** |        **1.00** |
 ManualBulkCopy | 1000  |  12.30 ms |  2.534 ms |  1.676 ms |  0.55 |    0.08 |   477.17 KB |        0.17 |
 GaroaBulk      | 1000  |  11.26 ms |  0.832 ms |  0.550 ms |  0.50 |    0.03 |   111.56 KB |        0.04 |
                |       |           |           |           |       |         |             |             |
 **Dapper**         | **10000** | **152.50 ms** | **28.568 ms** | **17.001 ms** |  **1.01** |    **0.14** | **27936.61 KB** |        **1.00** |
 ManualBulkCopy | 10000 | 100.22 ms | 22.073 ms | 14.600 ms |  0.66 |    0.11 |  5260.66 KB |        0.19 |
 GaroaBulk      | 10000 |  83.31 ms | 17.228 ms | 11.395 ms |  0.55 |    0.09 |  1060.78 KB |        0.04 |
