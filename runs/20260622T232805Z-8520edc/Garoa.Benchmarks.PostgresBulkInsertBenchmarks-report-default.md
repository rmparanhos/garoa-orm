
BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2
  Job-HUGUFK : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

 Method    | Rows  | Mean       | Error      | StdDev     | Ratio | RatioSD | Allocated   | Alloc Ratio |
---------- |------ |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|
 **Dapper**    | **1000**  |  **15.112 ms** |  **1.7735 ms** |  **0.9276 ms** |  **1.00** |    **0.08** |   **2655.9 KB** |        **1.00** |
 GaroaBulk | 1000  |   7.541 ms |  0.1501 ms |  0.0785 ms |  0.50 |    0.03 |    73.88 KB |        0.03 |
           |       |            |            |            |       |         |             |             |
 **Dapper**    | **10000** | **127.542 ms** | **61.6903 ms** | **40.8043 ms** |  **1.09** |    **0.46** | **26542.58 KB** |        **1.00** |
 GaroaBulk | 10000 |  48.017 ms | 34.4366 ms | 22.7777 ms |  0.41 |    0.23 |   708.05 KB |        0.03 |
