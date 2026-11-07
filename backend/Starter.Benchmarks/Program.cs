using BenchmarkDotNet.Running;

// Run in Release, never Debug: dotnet run -c Release --project Starter.Benchmarks
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
