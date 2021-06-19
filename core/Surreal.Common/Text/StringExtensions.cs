using System;
using System.Text;

namespace Surreal.Text
{
  public static class StringExtensions
  {
    public static string Lerp(this string source, string target, float amount)
    {
      if (amount <= 0f) return source;
      if (amount >= 1f) return target;
      if (source == target) return target;

      var sourceLength = (int) MathF.Ceiling(source.Length * amount);
      var targetLength = (int) MathF.Ceiling(target.Length * amount);

      var head = target.Substring(0, targetLength);
      var tail = source.Substring(sourceLength, source.Length - sourceLength);

      return head + tail;
    }

    public static StringBuilder AppendWithSeparator(this StringBuilder builder, string value, string seperator)
    {
      if (builder.Length > 0)
      {
        builder.Append(seperator);
      }

      builder.Append(value);

      return builder;
    }

    public static string Truncate(this string message, int length, string suffix = "...")
    {
      if (message.Length > length)
      {
        return $"{message[..length]}{suffix}";
      }

      return message;
    }

    public static string GetFullNameWithoutGenerics(this Type type)
    {
      return RemoveGenerics(type.FullName ?? string.Empty);
    }

    private static string RemoveGenerics(string value)
    {
      var index = value.IndexOf('`');

      return index == -1 ? value : value[..index];
    }
  }
}