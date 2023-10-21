namespace Surreal.Utilities;

/// <summary>
/// Static utilities for type reflection.
/// </summary>
public static class ReflectionExtensions
{
  /// <summary>
  /// Tries to get the custom attribute of the given type from the given member, optionally allowing for inherited attributes.
  /// </summary>
  public static bool TryGetCustomAttribute<T>(this ICustomAttributeProvider type, [NotNullWhen(true)] out T result,
    bool inherit = false)
    where T : Attribute
  {
    var attributes = type.GetCustomAttributes(typeof(T), inherit);

    if (attributes.Length > 1)
    {
      throw new AmbiguousMatchException($"Multiple attributes of type {typeof(T).Name} found on {type}.");
    }

    if (attributes.Length <= 0)
    {
      result = default!;
      return false;
    }

    result = (T)attributes[0];
    return true;
  }

  /// <summary>
  /// Tries to get all custom attributes of the given type from the given member, optionally allowing for inherited attributes.
  /// </summary>
  public static bool TryGetCustomAttributes<T>(this ICustomAttributeProvider type, out T[] result, bool inherit = false)
    where T : Attribute
  {
    var attributes = type.GetCustomAttributes(typeof(T), inherit);

    if (attributes.Length <= 0)
    {
      result = Array.Empty<T>();
      return false;
    }

    result = attributes.Cast<T>().ToArray();
    return true;
  }

  /// <summary>
  /// Finds all instance methods of a type, all the way up the type hierarchy.
  /// </summary>
  public static IEnumerable<MethodInfo> GetHierarchyMethods(this Type type)
  {
    const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

    IEnumerable<MethodInfo> methods = type.GetMethods(flags);

    if (type.BaseType != null)
    {
      methods = methods.Concat(GetHierarchyMethods(type.BaseType));
    }

    return methods;
  }
}
