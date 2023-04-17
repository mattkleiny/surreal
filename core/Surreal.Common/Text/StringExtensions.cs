namespace Surreal.Text;

/// <summary>
/// General purpose string extensions.
/// </summary>
public static class StringExtensions
{
  public static StringBuilder AppendIndent(this StringBuilder builder, int indentLevel)
  {
    return builder.Append(new string('\t', Math.Max(0, indentLevel)));
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

  public static StringBuilder AppendSpaced(this StringBuilder builder, string? value)
  {
    return builder.Append(value).Append(value != null ? " " : null);
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
