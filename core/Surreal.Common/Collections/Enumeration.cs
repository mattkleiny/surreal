using System.Reflection;

namespace Surreal.Collections;

/// <summary>
/// A structured enumeration with support for lookups and complex objects.
/// </summary>
[RequiresUnreferencedCode("Discovers types via reflection")]
public abstract record Enumeration<T>
  where T : Enumeration<T>
{
  public static ImmutableList<T> All { get; } = DiscoverValues();

  private static ImmutableList<T> DiscoverValues()
  {
    var builder = ImmutableList.CreateBuilder<T>();

    foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static))
    {
      if (property.PropertyType == typeof(T))
      {
        builder.Add((T)property.GetValue(null)!);
      }
    }

    return builder.ToImmutable();
  }
}
