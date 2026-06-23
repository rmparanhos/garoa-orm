
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-XSHUOG : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method    | Rows  | Mean       | Error      | StdDev     | Median     | Ratio | RatioSD | Allocated   | Alloc Ratio |
---------- |------ |-----------:|-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**    | **1000**  |  **13.590 ms** |  **5.2886 ms** |  **3.4981 ms** |  **12.763 ms** |  **1.05** |    **0.35** |  **2656.09 KB** |        **1.00** |
 GaroaBulk | 1000  |   5.716 ms |  0.2708 ms |  0.1416 ms |   5.667 ms |  0.44 |    0.10 |    73.88 KB |        0.03 |
           |       |            |            |            |            |       |         |             |             |
 **Dapper**    | **10000** | **111.203 ms** | **50.8770 ms** | **33.6520 ms** | **103.677 ms** |  **1.09** |    **0.44** | **26544.92 KB** |        **1.00** |
 GaroaBulk | 10000 |  49.993 ms | 40.6437 ms | 26.8833 ms |  39.577 ms |  0.49 |    0.29 |   708.01 KB |        0.03 |
