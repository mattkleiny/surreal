using System.Reflection;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using AutoFixture.NUnit3;
using JetBrains.Annotations;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace Surreal;

/// <summary>Provides a customized AutoFixture for use in test method parameter injection.</summary>
[AttributeUsage(AttributeTargets.Method)]
internal class AutoFixtureAttribute : AutoDataAttribute
{
  public AutoFixtureAttribute()
    : base(BuildFixture)
  {
  }

  private static IFixture BuildFixture()
  {
    var fixture = new Fixture();

    fixture.Customize(new AutoNSubstituteCustomization
    {
      GenerateDelegates = true,
    });

    fixture.Customizations.Add(new DateOnlyGenerator());
    fixture.Customizations.Add(new TimeOnlyGenerator());

    foreach (var builder in DiscoverSpecimenBuilders())
    {
      fixture.Customizations.Add(builder);
    }

    return fixture;
  }

  private sealed class DateOnlyGenerator : SpecimenBuilder<DateOnly>
  {
    protected override DateOnly Create(ISpecimenContext context, string? name = null)
    {
      var random = Random.Shared;

      var year  = random.Next(2017, 2024);
      var month = random.Next(1, 12);
      var day   = random.Next(1, 28);

      return new DateOnly(year, month, day);
    }
  }

  private sealed class TimeOnlyGenerator : SpecimenBuilder<TimeOnly>
  {
    protected override TimeOnly Create(ISpecimenContext context, string? name = null)
    {
      var random = Random.Shared;

      var hour   = random.Next(0, 23);
      var minute = random.Next(0, 59);
      var second = random.Next(0, 59);

      return new TimeOnly(hour, minute, second);
    }
  }

  private static IEnumerable<ISpecimenBuilder> DiscoverSpecimenBuilders()
  {
    return from type in Assembly.GetExecutingAssembly().GetTypes()
      where typeof(ISpecimenBuilder).IsAssignableFrom(type)
      where type.GetCustomAttribute<RegisterSpecimenBuilderAttribute>() != null
      where type.IsClass && !type.IsAbstract
      select (ISpecimenBuilder) Activator.CreateInstance(type)!;
  }
}

/// <summary>Registers the associated type as a <see cref="ISpecimenBuilder"/>.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
internal sealed class RegisterSpecimenBuilderAttribute : Attribute
{
}

/// <summary>Base class for any <see cref="ISpecimenBuilder"/> for <see cref="T"/> which handles different request sources.</summary>
internal abstract class SpecimenBuilder<T> : ISpecimenBuilder
  where T : notnull
{
  protected abstract T Create(ISpecimenContext context, string? name = null);

  object ISpecimenBuilder.Create(object request, ISpecimenContext context)
  {
    return request switch
    {
      ParameterInfo { Name: var name, ParameterType: var type } when type == typeof(T) => Create(context, name),
      PropertyInfo { Name: var name, PropertyType: var type } when type == typeof(T)   => Create(context, name),
      SeededRequest { Request: Type type } when type == typeof(T)                      => Create(context),

      _ => new NoSpecimen(),
    };
  }
}

/// <summary>A NUnit test attribute that benchmarks a method over multiple iterations.</summary>
[AttributeUsage(AttributeTargets.Method)]
internal class BenchmarkAttribute : PropertyAttribute, IWrapSetUpTearDown
{
  public float Milliseconds { get; set; } = 1000f;
  public int   Iterations   { get; set; } = 4;

  public TestCommand Wrap(TestCommand command)
  {
    return new BenchmarkCommand(command, Iterations, Milliseconds);
  }

  /// <summary>Wraps the test execution to allow measuring time taken.</summary>
  private sealed class BenchmarkCommand : DelegatingTestCommand
  {
    private readonly int   maxIterations;
    private readonly float thresholdMs;

    public BenchmarkCommand(TestCommand innerCommand, int maxIterations, float thresholdMs)
      : base(innerCommand)
    {
      this.maxIterations = maxIterations;
      this.thresholdMs   = thresholdMs;
    }

    public override TestResult Execute(TestExecutionContext context)
    {
      var iteration = 0;
      var stopwatch  = new Stopwatch();

      while (true)
      {
        stopwatch.Restart();
        var result = innerCommand.Execute(context);
        stopwatch.Stop();

        context.OutWriter.WriteLine($"Iteration {iteration} took {stopwatch.Elapsed:g}");

        if (stopwatch.Elapsed.TotalMilliseconds > thresholdMs)
        {
          result.SetResult(ResultState.Failure, $"Execution exceeded maximum benchmark time of {thresholdMs}ms");

          if (iteration++ < maxIterations)
          {
            continue;
          }
        }

        return result;
      }
    }
  }
}
