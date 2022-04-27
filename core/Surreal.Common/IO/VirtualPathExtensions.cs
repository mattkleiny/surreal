using System.IO.MemoryMappedFiles;
using System.Text.Json;
using System.Xml.Linq;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>Static extensions for working with <see cref="VirtualPath"/>s.</summary>
public static class VirtualPathExtensions
{
  private static readonly Encoding DefaultEncoding = Encoding.UTF8;

  public static bool SupportsWatching(this VirtualPath path)
    => path.GetFileSystem().SupportsWatcher;

  public static bool SupportsMemoryMapping(this VirtualPath path)
    => path.GetFileSystem().SupportsMemoryMapping;

  public static IFileSystem GetFileSystem(this VirtualPath path)
    => FileSystem.GetForScheme(path.Scheme.ToString())!;

  public static VirtualPath Resolve(this VirtualPath path, params string[] name)
    => path.GetFileSystem().Resolve(path, name);

  public static ValueTask<bool> ExistsAsync(this VirtualPath path)
    => path.GetFileSystem().ExistsAsync(path.Target.ToString());

  public static ValueTask<bool> IsFileAsync(this VirtualPath path)
    => path.GetFileSystem().IsFileAsync(path.Target.ToString());

  public static ValueTask<bool> IsDirectoryAsync(this VirtualPath path)
    => path.GetFileSystem().IsDirectoryAsync(path.Target.ToString());

  public static ValueTask<Size> GetSizeAsync(this VirtualPath path)
    => path.GetFileSystem().GetSizeAsync(path.Target.ToString());

  public static ValueTask<Stream> OpenInputStreamAsync(this VirtualPath path)
    => path.GetFileSystem().OpenInputStreamAsync(path.Target.ToString());

  public static ValueTask<Stream> OpenOutputStreamAsync(this VirtualPath path)
    => path.GetFileSystem().OpenOutputStreamAsync(path.Target.ToString());

  public static MemoryMappedFile OpenMemoryMappedFile(this VirtualPath path, int offset, int length)
    => path.GetFileSystem().OpenMemoryMappedFile(path.Target.ToString(), offset, length);

  public static IPathWatcher Watch(this VirtualPath path)
    => path.GetFileSystem().WatchPath(path);

  public static ValueTask<VirtualPath[]> EnumerateAsync(this VirtualPath path, string wildcard)
    => path.GetFileSystem().EnumerateAsync(path.Target.ToString(), wildcard);

  public static VirtualPath ChangeExtension(this VirtualPath path, string newExtension)
    => path with { Target = Path.ChangeExtension(path.Target.ToString(), newExtension) };

  public static VirtualPath GetDirectory(this VirtualPath path)
    => path with { Target = Path.GetDirectoryName(path.Target.ToSpan()) };

  public static async ValueTask CopyToAsync(this VirtualPath from, VirtualPath to, CancellationToken cancellationToken = default)
  {
    await using var input = await from.OpenInputStreamAsync();
    await using var output = await to.OpenOutputStreamAsync();

    await input.CopyToAsync(output, cancellationToken);
  }

  public static async ValueTask<byte[]> ReadAllBytesAsync(this VirtualPath path, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();
    await using var buffer = new MemoryStream();

    await stream.CopyToAsync(buffer, cancellationToken);

    return buffer.ToArray();
  }

  public static async ValueTask WriteAllBytesAsync(this VirtualPath path, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenOutputStreamAsync();

    await stream.WriteAsync(data, cancellationToken);
    await stream.FlushAsync(cancellationToken);
  }

  public static async ValueTask<string> ReadAllTextAsync(this VirtualPath path, Encoding? encoding = default, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();
    using var reader = new StreamReader(stream, encoding ?? DefaultEncoding);

    return await reader.ReadToEndAsync();
  }

  public static async ValueTask WriteAllTextAsync(this VirtualPath path, string text, Encoding? encoding = default, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenOutputStreamAsync();
    await using var writer = new StreamWriter(stream, encoding ?? DefaultEncoding);

    await writer.WriteAsync(text.AsMemory(), cancellationToken);
    await writer.FlushAsync();
  }

  public static ValueTask SerializeBinaryAsync<T>(this VirtualPath path, T value, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public static ValueTask<T> DeserializeBinaryAsync<T>(this VirtualPath path, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public static async ValueTask SerializeJsonAsync<T>(this VirtualPath path, T value, CancellationToken cancellationToken = default)
    where T : class
  {
    await using var stream = await path.OpenOutputStreamAsync();

    await JsonSerializer.SerializeAsync(stream, value, cancellationToken: cancellationToken);
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

    await XElement.LoadAsync(stream, LoadOptions.None, cancellationToken);

    throw new NotImplementedException();
  }

  public static async ValueTask<T> DeserializeXmlAsync<T>(this VirtualPath path, CancellationToken cancellationToken = default)
    where T : class
  {
    await using var stream = await path.OpenInputStreamAsync();

    await XElement.LoadAsync(stream, LoadOptions.None, cancellationToken);

    throw new NotImplementedException();
  }
}
