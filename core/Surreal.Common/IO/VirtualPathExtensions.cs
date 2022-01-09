using System.Text.Json;
using System.Xml.Linq;
using Surreal.IO.Binary;
using Surreal.IO.Xml;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>Static extensions for working with <see cref="VirtualPath"/>s.</summary>
public static class VirtualPathExtensions
{
  private static readonly Encoding DefaultEncoding = Encoding.UTF8;

  public static IFileSystem GetFileSystem(this VirtualPath path)
  {
    return FileSystem.GetForScheme(path.Scheme.ToString())!;
  }

  public static VirtualPath Resolve(this VirtualPath path, params string[] name) => path.GetFileSystem().Resolve(path.Target.ToString()!, name);

  public static ValueTask<bool>   ExistsAsync(this VirtualPath path)           => path.GetFileSystem().ExistsAsync(path.Target.ToString()!);
  public static ValueTask<bool>   IsFileAsync(this VirtualPath path)           => path.GetFileSystem().IsFileAsync(path.Target.ToString()!);
  public static ValueTask<bool>   IsDirectoryAsync(this VirtualPath path)      => path.GetFileSystem().IsDirectoryAsync(path.Target.ToString()!);
  public static ValueTask<Size>   GetSizeAsync(this VirtualPath path)          => path.GetFileSystem().GetSizeAsync(path.Target.ToString()!);
  public static ValueTask<Stream> OpenInputStreamAsync(this VirtualPath path)  => path.GetFileSystem().OpenInputStreamAsync(path.Target.ToString()!);
  public static ValueTask<Stream> OpenOutputStreamAsync(this VirtualPath path) => path.GetFileSystem().OpenOutputStreamAsync(path.Target.ToString()!);

  public static IPathWatcher Watch(this VirtualPath path) => path.GetFileSystem().WatchPath(path);

  public static ValueTask<VirtualPath[]> EnumerateAsync(this VirtualPath path, string wildcard)
  {
    return path.GetFileSystem().EnumerateAsync(path.Target.ToString()!, wildcard);
  }

  public static async ValueTask CopyToAsync(this VirtualPath from, VirtualPath to)
  {
    await using var input  = await from.OpenInputStreamAsync();
    await using var output = await to.OpenOutputStreamAsync();

    await input.CopyToAsync(output);
  }

  public static async ValueTask<byte[]> ReadAllBytesAsync(this VirtualPath path)
  {
    await using var stream = await path.OpenInputStreamAsync();
    await using var buffer = new MemoryStream();

    await stream.CopyToAsync(buffer);

    return buffer.ToArray();
  }

  public static async ValueTask WriteAllBytesAsync(this VirtualPath path, ReadOnlyMemory<byte> data)
  {
    await using var stream = await path.OpenOutputStreamAsync();

    await stream.WriteAsync(data);
    await stream.FlushAsync();
  }

  public static async ValueTask<string> ReadAllTextAsync(this VirtualPath path, Encoding? encoding = default)
  {
    await using var stream = await path.OpenInputStreamAsync();
    using var       reader = new StreamReader(stream, encoding ?? DefaultEncoding);

    return await reader.ReadToEndAsync();
  }

  public static async ValueTask WriteAllTextAsync(this VirtualPath path, string text, Encoding? encoding = default)
  {
    await using var stream = await path.OpenOutputStreamAsync();
    await using var writer = new StreamWriter(stream, encoding ?? DefaultEncoding);

    await writer.WriteAsync(text);
    await writer.FlushAsync();
  }

  public static async ValueTask SerializeBinaryAsync<T>(this VirtualPath path, T value, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenOutputStreamAsync();
    await using var writer = new StreamBinaryWriter(stream);

    await BinarySerializer.SerializeAsync(value, writer, cancellationToken);
  }

  public static async ValueTask<T> DeserializeBinaryAsync<T>(this VirtualPath path, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();
    await using var reader = new StreamBinaryReader(stream);

    return await BinarySerializer.DeserializeAsync<T>(reader, cancellationToken);
  }

  public static async ValueTask SerializeJsonAsync<T>(this VirtualPath path, T value)
    where T : class
  {
    await using var stream = await path.OpenOutputStreamAsync();

    await JsonSerializer.SerializeAsync(stream, value);
  }

  public static async ValueTask<object> DeserializeJsonAsync(this VirtualPath path, Type type, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    var result = await JsonSerializer.DeserializeAsync(stream, type, cancellationToken: cancellationToken);
    if (result == null)
    {
      throw new JsonException($"Failed to parse {type} from stream");
    }

    return result;
  }

  public static async ValueTask<T> DeserializeJsonAsync<T>(this VirtualPath path, CancellationToken cancellationToken = default)
    where T : class
  {
    await using var stream = await path.OpenInputStreamAsync();

    var result = await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
    if (result == null)
    {
      throw new JsonException($"Failed to parse {typeof(T)} from stream");
    }

    return result;
  }

  public static async ValueTask<object> DeserializeXmlAsync(this VirtualPath path, Type type, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    var element = await XElement.LoadAsync(stream, LoadOptions.None, cancellationToken);

    return await XmlSerializer.DeserializeAsync(type, element, cancellationToken);
  }

  public static async ValueTask<T> DeserializeXmlAsync<T>(this VirtualPath path, CancellationToken cancellationToken = default)
    where T : class
  {
    await using var stream = await path.OpenInputStreamAsync();

    var element = await XElement.LoadAsync(stream, LoadOptions.None, cancellationToken);

    return await XmlSerializer.DeserializeAsync<T>(element, cancellationToken);
  }
}
