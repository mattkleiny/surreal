using System.IO.MemoryMappedFiles;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>
/// Static extensions for working with <see cref="VirtualPath" />s.
/// </summary>
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

  public static VirtualPath[] Enumerate(this VirtualPath path, string wildcard)
    => path.GetFileSystem().Enumerate(path.Target.ToString(), wildcard);

  public static ValueTask<VirtualPath[]> EnumerateAsync(this VirtualPath path, string wildcard)
    => path.GetFileSystem().EnumerateAsync(path.Target.ToString(), wildcard);

  public static bool Exists(this VirtualPath path)
    => path.GetFileSystem().Exists(path.Target.ToString());

  public static ValueTask<bool> ExistsAsync(this VirtualPath path)
    => path.GetFileSystem().ExistsAsync(path.Target.ToString());

  public static bool IsFile(this VirtualPath path)
    => path.GetFileSystem().IsFile(path.Target.ToString());

  public static ValueTask<bool> IsFileAsync(this VirtualPath path)
    => path.GetFileSystem().IsFileAsync(path.Target.ToString());

  public static bool IsDirectory(this VirtualPath path)
    => path.GetFileSystem().IsDirectory(path.Target.ToString());

  public static ValueTask<bool> IsDirectoryAsync(this VirtualPath path)
    => path.GetFileSystem().IsDirectoryAsync(path.Target.ToString());

  public static Size GetSize(this VirtualPath path)
    => path.GetFileSystem().GetSize(path.Target.ToString());

  public static ValueTask<Size> GetSizeAsync(this VirtualPath path)
    => path.GetFileSystem().GetSizeAsync(path.Target.ToString());

  public static Stream OpenInputStream(this VirtualPath path)
    => path.GetFileSystem().OpenInputStream(path.Target.ToString());

  public static ValueTask<Stream> OpenInputStreamAsync(this VirtualPath path)
    => path.GetFileSystem().OpenInputStreamAsync(path.Target.ToString());

  public static Stream OpenOutputStream(this VirtualPath path)
    => path.GetFileSystem().OpenOutputStream(path.Target.ToString());

  public static ValueTask<Stream> OpenOutputStreamAsync(this VirtualPath path)
    => path.GetFileSystem().OpenOutputStreamAsync(path.Target.ToString());

  public static MemoryMappedFile OpenMemoryMappedFile(this VirtualPath path)
    => path.GetFileSystem().OpenMemoryMappedFile(path.Target.ToString());

  public static IPathWatcher Watch(this VirtualPath path)
    => path.GetFileSystem().WatchPath(path);

  public static VirtualPath ChangeExtension(this VirtualPath path, string newExtension)
  {
    return path with
    {
      Target = Path.ChangeExtension(path.Target.ToString(), newExtension)
    };
  }

  public static VirtualPath GetDirectory(this VirtualPath path)
  {
    return path with
    {
      Target = Path.GetDirectoryName(path.Target.ToSpan())
    };
  }

  public static async ValueTask CopyToAsync(this VirtualPath from, VirtualPath to, CancellationToken cancellationToken = default)
  {
    await using var input = await from.OpenInputStreamAsync();
    await using var output = await to.OpenOutputStreamAsync();

    await input.CopyToAsync(output, cancellationToken);
  }

  public static byte[] ReadAllBytes(this VirtualPath path)
  {
    using var stream = path.OpenInputStream();
    using var buffer = new MemoryStream();

    stream.CopyTo(buffer);

    return buffer.ToArray();
  }

  public static async ValueTask<byte[]> ReadAllBytesAsync(this VirtualPath path, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();
    await using var buffer = new MemoryStream();

    await stream.CopyToAsync(buffer, cancellationToken);

    return buffer.ToArray();
  }

  public static void WriteAllBytes(this VirtualPath path, ReadOnlySpan<byte> data)
  {
    using var stream = path.OpenOutputStream();

    stream.Write(data);
    stream.Flush();
  }

  public static async ValueTask WriteAllBytesAsync(this VirtualPath path, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenOutputStreamAsync();

    await stream.WriteAsync(data, cancellationToken);
    await stream.FlushAsync(cancellationToken);
  }

  public static string ReadAllText(this VirtualPath path)
  {
    return ReadAllText(path, DefaultEncoding);
  }

  public static string ReadAllText(this VirtualPath path, Encoding encoding)
  {
    using var stream = path.OpenInputStream();
    using var reader = new StreamReader(stream, encoding);

    return reader.ReadToEnd();
  }

  public static ValueTask<string> ReadAllTextAsync(this VirtualPath path, CancellationToken cancellationToken = default)
  {
    return ReadAllTextAsync(path, DefaultEncoding, cancellationToken);
  }

  public static async ValueTask<string> ReadAllTextAsync(this VirtualPath path, Encoding encoding, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();
    using var reader = new StreamReader(stream, encoding);

    return await reader.ReadToEndAsync(cancellationToken);
  }

  public static void WriteAllText(this VirtualPath path, string text)
  {
    WriteAllText(path, text, DefaultEncoding);
  }

  public static void WriteAllText(this VirtualPath path, string text, Encoding encoding)
  {
    using var stream = path.OpenOutputStream();
    using var writer = new StreamWriter(stream, encoding);

    writer.Write(text);
    writer.Flush();
  }

  public static ValueTask WriteAllTextAsync(this VirtualPath path, string text, CancellationToken cancellationToken = default)
  {
    return WriteAllTextAsync(path, text, DefaultEncoding, cancellationToken);
  }

  public static async ValueTask WriteAllTextAsync(this VirtualPath path, string text, Encoding encoding, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenOutputStreamAsync();
    await using var writer = new StreamWriter(stream, encoding);

    await writer.WriteAsync(text.AsMemory(), cancellationToken);
    await writer.FlushAsync();
  }

  public static void Serialize<T>(this VirtualPath path, T value, FileFormat format)
    where T : class
  {
    using var stream = path.OpenOutputStream();

    format.Serialize(stream, value);
  }

  public static async ValueTask SerializeAsync<T>(this VirtualPath path, T value, FileFormat format, CancellationToken cancellationToken = default)
    where T : class
  {
    await using var stream = await path.OpenOutputStreamAsync();

    await format.SerializeAsync(stream, value, cancellationToken);
  }

  public static T Deserialize<T>(this VirtualPath path, FileFormat format)
    where T : class
  {
    using var stream = path.OpenInputStream();

    var result = format.Deserialize<T>(stream);
    if (result == null)
    {
      throw new InvalidDataException($"Failed to parse {typeof(T)} from stream");
    }

    return result;
  }

  public static async ValueTask<T> DeserializeAsync<T>(this VirtualPath path, FileFormat format, CancellationToken cancellationToken = default)
    where T : class
  {
    await using var stream = await path.OpenInputStreamAsync();

    var result = await format.DeserializeAsync<T>(stream, cancellationToken);
    if (result == null)
    {
      throw new InvalidDataException($"Failed to parse {typeof(T)} from stream");
    }

    return result;
  }

  public static object Deserialize(this VirtualPath path, Type type, FileFormat format)
  {
    using var stream = path.OpenInputStream();

    var result = format.Deserialize(stream, type);
    if (result == null)
    {
      throw new InvalidDataException($"Failed to parse {type} from stream");
    }

    return result;
  }

  public static async ValueTask<object> DeserializeAsync(this VirtualPath path, Type type, FileFormat format, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    var result = await format.DeserializeAsync(stream, type, cancellationToken: cancellationToken);
    if (result == null)
    {
      throw new InvalidDataException($"Failed to parse {type} from stream");
    }

    return result;
  }
}
