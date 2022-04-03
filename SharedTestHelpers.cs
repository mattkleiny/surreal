using System.Reflection;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using AutoFixture.NUnit3;
using JetBrains.Annotations;
using Surreal.Mathematics;

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
    fixture.Customizations.Add(new IntRangeGenerator());
    fixture.Customizations.Add(new FloatRangeGenerator());
    fixture.Customizations.Add(new TimeSpanRangeGenerator());

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

  private sealed class TimeSpanRangeGenerator : SpecimenBuilder<TimeSpanRange>
  {
    protected override TimeSpanRange Create(ISpecimenContext context, string? name = null)
    {
      var random = Random.Shared;

      var value1 = random.NextTimeSpan();
      var value2 = random.NextTimeSpan();

      var min = value1 < value2 ? value1 : value2;
      var max = value1 > value2 ? value1 : value2;

      return new TimeSpanRange(min, max);
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
