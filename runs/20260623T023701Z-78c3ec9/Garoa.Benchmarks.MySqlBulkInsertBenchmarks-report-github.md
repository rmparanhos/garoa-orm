```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-XSHUOG : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method    | Rows  | Mean       | Error      | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|---------- |------ |-----------:|-----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**    | **1000**  |  **18.977 ms** |  **14.011 ms** |  **8.338 ms** |  **1.16** |    **0.65** |   **2794.9 KB** |        **1.00** |
| GaroaBulk | 1000  |   9.744 ms |   2.274 ms |  1.189 ms |  0.60 |    0.22 |   111.53 KB |        0.04 |
|           |       |            |            |           |       |         |             |             |
| **Dapper**    | **10000** | **286.660 ms** | **164.123 ms** | **97.667 ms** |  **1.12** |    **0.54** | **27936.61 KB** |        **1.00** |
| GaroaBulk | 10000 | 193.355 ms | 124.848 ms | 65.298 ms |  0.75 |    0.36 |  1060.75 KB |        0.04 |
