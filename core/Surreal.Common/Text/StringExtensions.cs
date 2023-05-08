namespace Surreal.Text;

/// <summary>
/// General purpose string extensions.
/// </summary>
public static class StringExtensions
{
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
