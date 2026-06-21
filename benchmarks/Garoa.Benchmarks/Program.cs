using BenchmarkDotNet.Running;

// Entry point. Run with e.g.:
//   dotnet run -c Release -- --filter '*' --exporters json
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

// Named so BenchmarkSwitcher can reference the assembly via typeof(Program).
public sealed partial class Program;
