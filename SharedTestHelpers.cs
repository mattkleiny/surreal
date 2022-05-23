using System.Reflection;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using JetBrains.Annotations;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using Surreal.Mathematics;

namespace Surreal;

/// <summary>Provides a customized AutoFixture for use in test method parameter injection.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method)]
[RequiresUnreferencedCode("Discovers specimen builders via reflection")]
internal class AutoTestAttribute : Attribute, ISimpleTestBuilder, IImplyFixture
{
  private readonly Lazy<IFixture> fixture = new(BuildFixture);

  [SuppressMessage("Trimming", "IL2046:\'RequiresUnreferencedCodeAttribute\' annotations must match across all interface implementations or overrides.")]
  TestMethod ISimpleTestBuilder.BuildFrom(IMethodInfo method, Test? suite)
  {
    var builder = new NUnitTestCaseBuilder();
    var parameters = BuildParameters(method);

    var result = builder.BuildTestMethod(method, suite, parameters);

    result.Name = method.Name; // simplify method name

    return result;
  }

  private TestCaseParameters BuildParameters(IMethodInfo method)
  {
    var fixture = this.fixture.Value;
    var context = new SpecimenContext(fixture);

    var parameters = method.GetParameters();
    var result = new object[parameters.Length];

    for (var i = 0; i < parameters.Length; i++)
    {
      var parameter = parameters[i];

      if (parameter.GetType() == typeof(IFixture) ||
          parameter.GetType() == typeof(Fixture))
      {
        result[i] = fixture;
      }
      else
      {
        result[i] = fixture.Create(parameter.ParameterType, context);
      }
    }

    return new TestCaseParameters(result);
  }

  private static IFixture BuildFixture()
  {
    var fixture = new Fixture();

    fixture.Customize(new AutoNSubstituteCustomization
    {
      GenerateDelegates = true
    });

    fixture.Customizations.Add(new DateOnlyGenerator());
    fixture.Customizations.Add(new TimeOnlyGenerator());
    fixture.Customizations.Add(new IntRangeGenerator());
    fixture.Customizations.Add(new FloatRangeGenerator());

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

      var year = random.Next(2017, 2024);
      var month = random.Next(1, 12);
      var day = random.Next(1, 28);

      return new DateOnly(year, month, day);
    }
  }

  private sealed class TimeOnlyGenerator : SpecimenBuilder<TimeOnly>
  {
    protected override TimeOnly Create(ISpecimenContext context, string? name = null)
    {
      var random = Random.Shared;

      var hour = random.Next(0, 23);
      var minute = random.Next(0, 59);
      var second = random.Next(0, 59);

      return new TimeOnly(hour, minute, second);
    }
  }

  private sealed class IntRangeGenerator : SpecimenBuilder<IntRange>
  {
    protected override IntRange Create(ISpecimenContext context, string? name = null)
    {
      var random = Random.Shared;

      var value1 = random.Next();
      var value2 = random.Next();

      var min = Math.Min(value1, value2);
      var max = Math.Max(value1, value2);

      return new IntRange(min, max);
    }
  }

  private sealed class FloatRangeGenerator : SpecimenBuilder<FloatRange>
  {
    protected override FloatRange Create(ISpecimenContext context, string? name = null)
    {
      var random = Random.Shared;

      var value1 = random.NextFloat();
      var value2 = random.NextFloat();

      var min = MathF.Min(value1, value2);
      var max = MathF.Max(value1, value2);

      return new FloatRange(min, max);
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

      _ => new NoSpecimen()
    };
  }
}
