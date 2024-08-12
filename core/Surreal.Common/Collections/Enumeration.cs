namespace Surreal.Collections;

/// <summary>
/// A structured enumeration with support for lookups and complex objects.
/// </summary>
public abstract record Enumeration<T>
  where T : Enumeration<T>
{
  /// <summary>
  /// All of the enumeration values.
  /// </summary>
  public static ImmutableList<T> All { get; } = Enumeration.DiscoverAll<T>(typeof(T));
}

/// <summary>
/// Helpers for working with enumerations.
/// </summary>
public static class Enumeration
{
  /// <summary>
  /// Discovers all the enumeration values of the specified type.
  /// </summary>
  public static ImmutableList<T> DiscoverAll<T>(Type type)
  {
    var builder = ImmutableList.CreateBuilder<T>();

    foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
    {
      if (typeof(T).IsAssignableFrom(property.PropertyType))
      {
        builder.Add((T)property.GetValue(null)!);
      }
    }

    return builder.ToImmutable();
  }
}
