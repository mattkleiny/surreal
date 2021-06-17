using System;

namespace Surreal.Utilities {
  public static class Reflection {
    public static bool TryCalculateTypeDistance(this Type type, Type baseType, out int distance) {
      if (type == baseType) {
        distance = 0;
        return true;
      }

      if (!baseType.IsAssignableFrom(type)) {
        distance = 0;
        return false;
      }

      distance = 0;

      while (type != baseType) {
        distance++;
        type = type.BaseType ?? throw new Exception("Don't know how we got here :shrug:");
      }

      return true;
    }
  }
}