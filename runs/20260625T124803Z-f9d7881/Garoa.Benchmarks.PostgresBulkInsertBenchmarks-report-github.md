```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-GNBIVL : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method     | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
|----------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
| **Dapper**     | **1000**  |  **15.599 ms** |  **5.8888 ms** |  **3.8951 ms** |  **1.05** |    **0.33** |  **2654.91 KB** |       **1.000** |
| ManualCopy | 1000  |   5.464 ms |  0.6226 ms |  0.3705 ms |  0.37 |    0.08 |     2.07 KB |       0.001 |
| GaroaBulk  | 1000  |   5.317 ms |  0.2587 ms |  0.1353 ms |  0.36 |    0.07 |     3.57 KB |       0.001 |
|            |       |            |            |            |       |         |             |             |
| **Dapper**     | **10000** | **132.647 ms** | **54.2586 ms** | **35.8887 ms** |  **1.07** |    **0.39** | **26543.52 KB** |       **1.000** |
| ManualCopy | 10000 |  41.455 ms | 23.1977 ms | 15.3438 ms |  0.33 |    0.15 |     3.71 KB |       0.000 |
| GaroaBulk  | 10000 |  40.980 ms | 18.9259 ms | 12.5183 ms |  0.33 |    0.13 |     5.45 KB |       0.000 |
