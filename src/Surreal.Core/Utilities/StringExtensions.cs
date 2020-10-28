using System;
using System.Text;

namespace Surreal.Utilities {
  public static class StringExtensions {
    public static StringBuilder AppendWithSeparator(this StringBuilder builder, string value, string seperator) {
      if (builder.Length > 0) {
        builder.Append(seperator);
      }

      builder.Append(value);

      return builder;
    }

    public static string GetFullNameWithoutGenerics(this Type type) {
      return RemoveGenerics(type.FullName ?? string.Empty);
    }

    private static string RemoveGenerics(string value) {
      var index = value.IndexOf('`');

      return index == -1 ? value : value.Substring(0, index);
    }
  }
}