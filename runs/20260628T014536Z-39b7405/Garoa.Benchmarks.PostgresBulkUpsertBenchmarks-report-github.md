```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-ERJCVT : .NET 8.0.28 (8.0.2826.26413), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  

```
| Method        | Rows  | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated   | Alloc Ratio |
|-------------- |------ |----------:|----------:|----------:|------:|--------:|------------:|------------:|
| **Dapper**        | **1000**  |  **16.47 ms** |  **2.510 ms** |  **1.494 ms** |  **1.01** |    **0.12** |  **2657.16 KB** |       **1.000** |
| ManualStaging | 1000  |  10.52 ms |  0.683 ms |  0.357 ms |  0.64 |    0.06 |     5.42 KB |       0.002 |
| GaroaBulk     | 1000  |  11.38 ms |  2.335 ms |  1.545 ms |  0.70 |    0.11 |     9.27 KB |       0.003 |
|               |       |           |           |           |       |         |             |             |
| **Dapper**        | **10000** | **132.57 ms** | **48.509 ms** | **28.867 ms** |  **1.03** |    **0.28** | **26553.98 KB** |       **1.000** |
| ManualStaging | 10000 |  61.75 ms | 19.704 ms | 11.725 ms |  0.48 |    0.12 |     5.52 KB |       0.000 |
| GaroaBulk     | 10000 |  68.55 ms | 23.716 ms | 15.687 ms |  0.53 |    0.15 |    10.91 KB |       0.000 |
