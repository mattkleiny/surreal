using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Surreal.Text;

namespace Surreal.IO;

/// <summary>Represents a path in the virtual file system.</summary>
[TypeConverter(typeof(VirtualPathTypeConverter))]
[JsonConverter(typeof(VirtualPathJsonConverter))]
public readonly record struct VirtualPath(StringSpan Scheme, StringSpan Target)
{
  private const string SchemeSeparator = "://";

  public string Extension => Path.GetExtension(Target.Source)!;

  /// <summary>Parses the given potential URI into a <see cref="VirtualPath" />.</summary>
  public static VirtualPath Parse(string uri)
  {
    if (StringSpan.TrySplit(uri, SchemeSeparator, out var result))
    {
      return new VirtualPath(result.Left, result.Right);
    }

    return new VirtualPath("local", uri);
  }

  public override string ToString()
  {
    return $"{Scheme}://{Target}";
  }

  public static implicit operator VirtualPath(string uri)
  {
    return Parse(uri);
  }

  /// <summary>The <see cref="TypeConverter" /> for <see cref="VirtualPath" />s.</summary>
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

  /// <summary>The <see cref="JsonConverter{T}" /> for <see cref="VirtualPath" />s.</summary>
  public class VirtualPathJsonConverter : JsonConverter<VirtualPath>
  {
    public override VirtualPath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      var value = reader.GetString();

      if (value == null)
      {
        return default;
      }

      return value;
    }

    public override void Write(Utf8JsonWriter writer, VirtualPath value, JsonSerializerOptions options)
    {
      writer.WriteStringValue(value.ToString());
    }
  }
}



