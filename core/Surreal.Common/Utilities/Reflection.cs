using System.Reflection;

namespace Surreal.Utilities;

public static class Reflection
{
  /// <summary>Finds all instance methods of a type, all the way up the hierarchy.</summary>
  public static IEnumerable<MethodInfo> GetFlattenedInstanceMethods(this Type type)
  {
    IEnumerable<MethodInfo> methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

    if (type.BaseType != null)
    {
      methods = methods.Concat(GetFlattenedInstanceMethods(type.BaseType));
    }

    return methods;
  }
}
