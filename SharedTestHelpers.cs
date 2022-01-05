using System.Reflection;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Kernel;
using AutoFixture.NUnit3;

namespace Surreal;

/// <summary>Provides a customized AutoFixture for use in test method parameter injection.</summary>
[AttributeUsage(AttributeTargets.Method)]
internal class AutoFixtureAttribute : AutoDataAttribute
{
  private static readonly NoSpecimen NoSpecimen = new();

  public AutoFixtureAttribute()
    : base(BuildFixture)
  {
  }

  private static IFixture BuildFixture()
  {
    var fixture = new Fixture();

    fixture.Customize(new AutoNSubstituteCustomization());
    fixture.Customizations.Add(new DateOnlyGenerator());
    fixture.Customizations.Add(new TimeOnlyGenerator());

    return fixture;
  }

  /// <summary>Base class for any <see cref="ISpecimenBuilder"/> for <see cref="T"/> which handles different request sources.</summary>
  private abstract class SpecimenBuilder<T> : ISpecimenBuilder
    where T : notnull
  {
    protected abstract T Create(string name);

    object ISpecimenBuilder.Create(object request, ISpecimenContext context)
    {
      return request switch
      {
        ParameterInfo { Name: var name, ParameterType: var type } when type == typeof(T) => Create(name!),
        PropertyInfo { Name: var name, PropertyType: var type } when type == typeof(T)   => Create(name),

        _ => NoSpecimen,
      };
    }
  }

  private sealed class DateOnlyGenerator : SpecimenBuilder<DateOnly>
  {
    protected override DateOnly Create(string name)
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
    protected override TimeOnly Create(string name)
    {
      var random = Random.Shared;

      var hour   = random.Next(0, 23);
      var minute = random.Next(0, 59);
      var second = random.Next(0, 59);

      return new TimeOnly(hour, minute, second);
    }
  }
}
