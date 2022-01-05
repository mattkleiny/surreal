using System.Reflection;

namespace Surreal.Collections;

#pragma warning disable CA1711 // deliberately suffixed with Enum

/// <summary>A structured enumeration with support for lookups and complex objects.</summary>
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
        builder.Add((T) property.GetValue(null)!);
      }
    }

    return builder.ToImmutable();
  }
}
