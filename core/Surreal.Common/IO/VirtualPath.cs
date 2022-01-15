using System.ComponentModel;
using System.Globalization;
using Surreal.Text;

namespace Surreal.IO;

/// <summary>Represents a path in the virtual file system.</summary>
[TypeConverter(typeof(VirtualPathTypeConverter))]
public readonly record struct VirtualPath(StringSpan Scheme, StringSpan Target)
{
  private const string SchemeSeparator = "://";

  public static VirtualPath Parse(string uri)
  {
    StringSpan scheme;
    StringSpan target;

    var index = uri.IndexOf(SchemeSeparator, StringComparison.Ordinal);
    if (index > -1)
    {
      scheme = uri.AsStringSpan(0, index);
      target = uri.AsStringSpan(index + SchemeSeparator.Length);
    }
    else
    {
      scheme = "local";
      target = uri;
    }

    return new VirtualPath(scheme, target);
  }

  public string Extension => Path.GetExtension(Target.Source)!;

  public override string ToString() => $"{Scheme}://{Target}";

  public static implicit operator VirtualPath(string uri) => Parse(uri);

  /// <summary>The <see cref="TypeConverter"/> for <see cref="VirtualPath"/>s.</summary>
  private sealed class VirtualPathTypeConverter : TypeConverter
  {
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
      return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
      return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      if (value == null || destinationType != typeof(string))
      {
        return base.ConvertTo(context, culture, value, destinationType);
      }

      return (VirtualPath) value.ToString()!;
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (value is not string raw)
      {
        return base.ConvertFrom(context, culture, value);
      }

      return (VirtualPath) raw;
    }
  }
}
