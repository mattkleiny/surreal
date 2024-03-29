﻿using System.Numerics;
using Surreal.Benchmarks;
using Surreal.Maths;

BenchmarkRunner.Run<VariantBenchmark>();

namespace Surreal.Benchmarks
{
  [MemoryDiagnoser]
  [DisassemblyDiagnoser]
  public class VariantBenchmark
  {
    private readonly Vector4 _vector;

    public VariantBenchmark()
    {
      var random = new Random(42);

      _vector = random.NextVector4(0f, 1f);
    }

    [Benchmark(Baseline = true)]
    public Vector4 Baseline() => PassNormal(_vector);

    [Benchmark]
    public Vector4 ByVariant() => PassByVariant(_vector);

    private Vector4 PassNormal(Vector4 vector)
    {
      return _vector + vector;
    }

    private Vector4 PassByVariant(Variant variant)
    {
      return _vector + variant.AsVector4();
    }
  }
}
