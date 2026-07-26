```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon 6973P-C, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-FDCKSM : .NET 8.0.29 (8.0.2926.32403), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **17.470 ms** |   **5.262 ms** |  **3.4808 ms** |  **1.04** |    **0.28** |  **2772.15 KB** |       **1.000** |
| ManualCopy | 1000  |   5.696 ms |   1.057 ms |  0.6991 ms |  0.34 |    0.07 |     2.73 KB |       0.001 |
| GaroaBulk  | 1000  |   4.846 ms |   1.611 ms |  0.9586 ms |  0.29 |    0.08 |     3.42 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **162.887 ms** | **122.263 ms** | **80.8692 ms** |  **1.41** |    **1.23** | **26544.08 KB** |       **1.000** |
| ManualCopy | 10000 |  39.209 ms |  27.026 ms | 16.0825 ms |  0.34 |    0.27 |     3.99 KB |       0.000 |
| GaroaBulk  | 10000 |  26.770 ms |  11.218 ms |  5.8670 ms |  0.23 |    0.16 |     4.73 KB |       0.000 |
