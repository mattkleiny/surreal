using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Surreal.Memory;
using Surreal.Text;

namespace Surreal.IO
{
  /// <summary>Represents a path in some virtual file system.</summary>
  [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
  public readonly struct Path : IEquatable<Path>
  {
    private const string SchemeSeparator = "://";

    public static Path Parse(string uri)
    {
      StringSpan scheme;
      StringSpan target;

      var index = uri.IndexOf(SchemeSeparator);
      if (index > -1)
      {
        scheme = uri.AsStringSpan(0, index);
        target = uri.AsStringSpan(index + SchemeSeparator.Length);
      }
      else
      {
        scheme = "local";
        target = default;
      }

      return new(scheme, target);
    }

    public Path(StringSpan scheme, StringSpan target)
    {
      Scheme = scheme;
      Target = target;
    }

    public void Deconstruct(out StringSpan scheme, out StringSpan target)
    {
      scheme = Scheme;
      target = Target;
    }

    public StringSpan Scheme    { get; }
    public StringSpan Target    { get; }
    public string     Extension => System.IO.Path.GetExtension(Target.Source)!;

    public override string ToString() => $"<{Scheme.ToString()}://{Target.ToString()}>";

    public          bool Equals(Path other)  => Scheme.Equals(other.Scheme) && Target.Equals(other.Target);
    public override bool Equals(object? obj) => obj is Path other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Scheme, Target);

    public static bool operator ==(Path left, Path right) => left.Equals(right);
    public static bool operator !=(Path left, Path right) => !left.Equals(right);

    public static implicit operator Path(string uri) => Parse(uri);
  }

  /// <summary>Static extensions for working with <see cref="Path"/>s.</summary>
  public static class PathExtensions
  {
    public static IFileSystem GetFileSystem(this Path path) => FileSystem.GetForScheme(path.Scheme.ToString()!)!;

    public static Path              Resolve(this Path path, params string[] name)   => path.GetFileSystem().Resolve(path.Target.ToString()!, name);
    public static ValueTask<bool>   ExistsAsync(this Path path)                     => path.GetFileSystem().ExistsAsync(path.Target.ToString()!);
    public static ValueTask<bool>   IsFileAsync(this Path path)                     => path.GetFileSystem().IsFileAsync(path.Target.ToString()!);
    public static ValueTask<bool>   IsDirectoryAsync(this Path path)                => path.GetFileSystem().IsDirectoryAsync(path.Target.ToString()!);
    public static ValueTask<Path[]> EnumerateAsync(this Path path, string wildcard) => path.GetFileSystem().EnumerateAsync(path.Target.ToString()!, wildcard);
    public static ValueTask<Size>   GetSizeAsync(this Path path)                    => path.GetFileSystem().GetSizeAsync(path.Target.ToString()!);

    public static ValueTask<Stream> OpenInputStreamAsync(this Path path)  => path.GetFileSystem().OpenInputStreamAsync(path.Target.ToString()!);
    public static ValueTask<Stream> OpenOutputStreamAsync(this Path path) => path.GetFileSystem().OpenOutputStreamAsync(path.Target.ToString()!);

    public static IPathWatcher Watch(this Path path) => path.GetFileSystem().WatchPath(path);

    public static async Task CopyToAsync(this Path from, Path to)
    {
      await using var input  = await from.OpenInputStreamAsync();
      await using var output = await to.OpenOutputStreamAsync();

      await input.CopyToAsync(output);
    }

    public static async Task<byte[]> ReadAllBytesAsync(this Path path)
    {
      await using var stream = await path.OpenInputStreamAsync();
      await using var buffer = new MemoryStream();

      await stream.CopyToAsync(buffer);

      return buffer.ToArray();
    }

    public static async Task WriteAllBytesAsync(this Path path, ReadOnlyMemory<byte> data)
    {
      await using var stream = await path.OpenOutputStreamAsync();

      await stream.WriteAsync(data);
      await stream.FlushAsync();
    }

    public static async Task<string> ReadAllTextAsync(this Path path, Encoding? encoding = default)
    {
      await using var stream = await path.OpenInputStreamAsync();
      using var       reader = new StreamReader(stream, encoding ?? Encoding.UTF8);

      return await reader.ReadToEndAsync();
    }

    public static async Task WriteAllTextAsync(this Path path, string text, Encoding? encoding = default)
    {
      await using var stream = await path.OpenOutputStreamAsync();
      await using var writer = new StreamWriter(stream, encoding ?? Encoding.UTF8);

      await writer.WriteAsync(text);
      await writer.FlushAsync();
    }

    public static async Task<T?> DeserializeJsonAsync<T>(this Path path)
        where T : class
    {
      await using var stream = await path.OpenInputStreamAsync();

      return await JsonSerializer.DeserializeAsync<T>(stream);
    }

    public static async Task<T> DeserializeXmlAsync<T>(this Path path)
        where T : class
    {
      await using var stream     = await path.OpenInputStreamAsync();
      var             serializer = new XmlSerializer(typeof(T));

      return (T) serializer.Deserialize(stream)!;
    }

    public static async Task SerializeJsonAsync<T>(this Path path, T value)
        where T : class
    {
      await using var stream = await path.OpenOutputStreamAsync();

      await JsonSerializer.SerializeAsync(stream, value);
    }

    public static async Task SerializeXmlAsync<T>(this Path path, T value)
        where T : class
    {
      await using var stream = await path.OpenOutputStreamAsync();

      var serializer = new XmlSerializer(typeof(T));

      serializer.Serialize(stream, value);
    }
  }
}
