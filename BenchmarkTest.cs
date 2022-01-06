using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace Surreal;

/// <summary>A NUnit test attribute that benchmarks a method over multiple iterations.</summary>
[AttributeUsage(AttributeTargets.Method)]
internal class BenchmarkAttribute : PropertyAttribute, IWrapSetUpTearDown
{
  public int ThresholdMs   { get; set; } = 1000;
  public int MaxIterations { get; set; } = 4;

  public TestCommand Wrap(TestCommand command)
  {
    return new BenchmarkCommand(command, MaxIterations, ThresholdMs);
  }

  /// <summary>Wraps the test execution to allow measuring time taken.</summary>
  private sealed class BenchmarkCommand : DelegatingTestCommand
  {
    private readonly int maxIterations;
    private readonly int thresholdMs;

    public BenchmarkCommand(TestCommand innerCommand, int maxIterations, int thresholdMs)
      : base(innerCommand)
    {
      this.maxIterations = maxIterations;
      this.thresholdMs   = thresholdMs;
    }

    public override TestResult Execute(TestExecutionContext context)
    {
      var iterations = 0;
      var stopwatch  = new Stopwatch();

      while (true)
      {
        stopwatch.Restart();
        var result = innerCommand.Execute(context);
        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > thresholdMs)
        {
          result.SetResult(ResultState.Failure, $"Execution exceeded maximum benchmark time of {thresholdMs}ms");

          if (iterations++ < maxIterations)
          {
            continue;
          }
        }

        return result;
      }
    }
  }
}
