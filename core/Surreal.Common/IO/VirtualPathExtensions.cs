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

  public static bool Exists(this VirtualPath path)
    => path.GetFileSystem().Exists(path.Target.ToString());

  public static bool IsFile(this VirtualPath path)
    => path.GetFileSystem().IsFile(path.Target.ToString());

  public static bool IsDirectory(this VirtualPath path)
    => path.GetFileSystem().IsDirectory(path.Target.ToString());

  public static Size GetSize(this VirtualPath path)
    => path.GetFileSystem().GetSize(path.Target.ToString());

  public static Stream OpenInputStream(this VirtualPath path)
    => path.GetFileSystem().OpenInputStream(path.Target.ToString());

  public static StreamReader OpenInputStreamReader(this VirtualPath path, Encoding? encoding = null)
    => new(path.GetFileSystem().OpenInputStream(path.Target.ToString()), encoding ?? DefaultEncoding);

  public static Stream OpenOutputStream(this VirtualPath path)
    => path.GetFileSystem().OpenOutputStream(path.Target.ToString());

  public static StreamWriter OpenOutputStreamWriter(this VirtualPath path, Encoding? encoding = null)
    => new(path.GetFileSystem().OpenOutputStream(path.Target.ToString()), encoding ?? DefaultEncoding);

  public static MemoryMappedFile OpenMemoryMappedFile(this VirtualPath path)
    => path.GetFileSystem().OpenMemoryMappedFile(path.Target.ToString());

  public static IPathWatcher Watch(this VirtualPath path, bool includeSubPaths = false)
    => path.GetFileSystem().WatchPath(path, includeSubPaths);

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
    await using var input = from.OpenInputStream();
    await using var output = to.OpenOutputStream();

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
    await using var stream = path.OpenInputStream();
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
    await using var stream = path.OpenOutputStream();

    await stream.WriteAsync(data, cancellationToken);
    await stream.FlushAsync(cancellationToken);
  }

  public static string ReadAllText(this VirtualPath path)
  {
    return ReadAllText(path, DefaultEncoding);
  }

  public static string ReadAllText(this VirtualPath path, Encoding encoding)
  {
    using var reader = path.OpenInputStreamReader(encoding);

    return reader.ReadToEnd();
  }

  public static ValueTask<string> ReadAllTextAsync(this VirtualPath path, CancellationToken cancellationToken = default)
  {
    return ReadAllTextAsync(path, DefaultEncoding, cancellationToken);
  }

  public static async ValueTask<string> ReadAllTextAsync(this VirtualPath path, Encoding encoding, CancellationToken cancellationToken = default)
  {
    using var reader = path.OpenInputStreamReader(encoding);

    return await reader.ReadToEndAsync(cancellationToken);
  }

  public static void WriteAllText(this VirtualPath path, string text)
  {
    WriteAllText(path, text, DefaultEncoding);
  }

  public static void WriteAllText(this VirtualPath path, string text, Encoding encoding)
  {
    using var writer = path.OpenOutputStreamWriter(encoding);

    writer.Write(text);
    writer.Flush();
  }

  public static ValueTask WriteAllTextAsync(this VirtualPath path, string text, CancellationToken cancellationToken = default)
  {
    return WriteAllTextAsync(path, text, DefaultEncoding, cancellationToken);
  }

  public static async ValueTask WriteAllTextAsync(this VirtualPath path, string text, Encoding encoding, CancellationToken cancellationToken = default)
  {
    await using var writer = path.OpenOutputStreamWriter(encoding);

    await writer.WriteAsync(text.AsMemory(), cancellationToken);
    await writer.FlushAsync(cancellationToken);
  }

  public static void Serialize<[MeansImplicitUse] T>(this VirtualPath path, T value, FileFormat format)
    where T : class
  {
    using var stream = path.OpenOutputStream();

    format.Serialize(stream, value);
  }

  public static async ValueTask SerializeAsync<[MeansImplicitUse] T>(this VirtualPath path, T value, FileFormat format, CancellationToken cancellationToken = default)
    where T : class
  {
    await using var stream = path.OpenOutputStream();

    await format.SerializeAsync(stream, value, cancellationToken);
  }

  public static T Deserialize<[MeansImplicitUse] T>(this VirtualPath path, FileFormat format)
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

  public static async ValueTask<T> DeserializeAsync<[MeansImplicitUse] T>(this VirtualPath path, FileFormat format, CancellationToken cancellationToken = default)
    where T : class
  {
    await using var stream = path.OpenInputStream();

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
    await using var stream = path.OpenInputStream();

    var result = await format.DeserializeAsync(stream, type, cancellationToken: cancellationToken);
    if (result == null)
    {
      throw new InvalidDataException($"Failed to parse {type} from stream");
    }

    return result;
  }
}
