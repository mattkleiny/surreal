using System.IO.MemoryMappedFiles;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>
/// Static extensions for working with <see cref="VirtualPath" />s.
/// </summary>
public static class VirtualPathExtensions
{
  private static readonly Encoding DefaultEncoding = Encoding.UTF8;

  /// <summary>
  /// Normalizes a <see cref="VirtualPath"/>.
  /// </summary>
  public static VirtualPath Normalize(this VirtualPath path)
  {
    return path with { Target = Path.GetFullPath(path.Target.ToString()) };
  }

  /// <summary>
  /// Gets the <see cref="IFileSystem"/> associated with the <see cref="VirtualPath" />.
  /// </summary>
  public static IFileSystem GetFileSystem(this VirtualPath path)
  {
    return FileSystem.Registry.GetByScheme(path.Scheme.ToString())!;
  }

  /// <summary>
  /// Determines if the <see cref="VirtualPath"/> supports watching.
  /// </summary>
  public static bool SupportsWatching(this VirtualPath path)
  {
    return path.GetFileSystem().SupportsWatcher;
  }

  /// <summary>
  /// Determines if the <see cref="VirtualPath"/> supports memory mapping.
  /// </summary>
  public static bool SupportsMemoryMapping(this VirtualPath path)
  {
    return path.GetFileSystem().SupportsMemoryMapping;
  }

  /// <summary>
  /// Resolves a <see cref="VirtualPath"/> relative to the current path.
  /// </summary>
  public static VirtualPath Resolve(this VirtualPath path, params string[] name)
  {
    return path.GetFileSystem().Resolve(path, name);
  }

  /// <summary>
  /// Enumerates the contents of a <see cref="VirtualPath"/>.
  /// </summary>
  public static VirtualPath[] Enumerate(this VirtualPath path, string wildcard)
  {
    return path.GetFileSystem().Enumerate(path.Target.ToString(), wildcard);
  }

  /// <summary>
  /// Determines if a <see cref="VirtualPath"/> exists.
  /// </summary>
  public static bool Exists(this VirtualPath path)
  {
    return path.GetFileSystem().Exists(path.Target.ToString());
  }

  /// <summary>
  /// Determines if a <see cref="VirtualPath"/> is a file.
  /// </summary>
  public static bool IsFile(this VirtualPath path)
  {
    return path.GetFileSystem().IsFile(path.Target.ToString());
  }

  /// <summary>
  /// Determines if a <see cref="VirtualPath"/> is a directory.
  /// </summary>
  public static bool IsDirectory(this VirtualPath path)
  {
    return path.GetFileSystem().IsDirectory(path.Target.ToString());
  }

  /// <summary>
  /// Gets the size of a <see cref="VirtualPath"/>.
  /// </summary>
  public static Size GetSize(this VirtualPath path)
  {
    return path.GetFileSystem().GetSize(path.Target.ToString());
  }

  /// <summary>
  /// Opens a <see cref="Stream"/> to a <see cref="VirtualPath"/>.
  /// </summary>
  public static Stream OpenInputStream(this VirtualPath path)
  {
    return path.GetFileSystem().OpenInputStream(path.Target.ToString());
  }

  /// <summary>
  /// Opens a <see cref="StreamReader"/> to a <see cref="VirtualPath"/>.
  /// </summary>
  public static StreamReader OpenInputStreamReader(this VirtualPath path, Encoding? encoding = null)
  {
    return new StreamReader(path.GetFileSystem().OpenInputStream(path.Target.ToString()), encoding ?? DefaultEncoding);
  }

  /// <summary>
  /// Opens a <see cref="Stream"/> to a <see cref="VirtualPath"/>.
  /// </summary>
  public static Stream OpenOutputStream(this VirtualPath path)
  {
    return path.GetFileSystem().OpenOutputStream(path.Target.ToString());
  }

  /// <summary>
  /// Opens a <see cref="StreamWriter"/> to a <see cref="VirtualPath"/>.
  /// </summary>
  public static StreamWriter OpenOutputStreamWriter(this VirtualPath path, Encoding? encoding = null)
  {
    return new StreamWriter(path.GetFileSystem().OpenOutputStream(path.Target.ToString()), encoding ?? DefaultEncoding);
  }

  /// <summary>
  /// Opens a <see cref="MemoryMappedFile"/> to a <see cref="VirtualPath"/>.
  /// </summary>
  public static MemoryMappedFile OpenMemoryMappedFile(this VirtualPath path)
  {
    return path.GetFileSystem().OpenMemoryMappedFile(path.Target.ToString());
  }

  /// <summary>
  /// Watches a <see cref="VirtualPath"/> for changes.
  /// </summary>
  public static IPathWatcher Watch(this VirtualPath path, bool includeSubPaths = false)
  {
    return path.GetFileSystem().WatchPath(path, includeSubPaths);
  }

  /// <summary>
  /// Converts a <see cref="VirtualPath"/> to an absolute path.
  /// </summary>
  public static string ToAbsolutePath(this VirtualPath path)
  {
    return path.GetFileSystem().ToAbsolutePath(path);
  }

  /// <summary>
  /// Changes the extension of a <see cref="VirtualPath"/>.
  /// </summary>
  public static VirtualPath ChangeExtension(this VirtualPath path, string newExtension)
  {
    return path with { Target = Path.ChangeExtension(path.Target.ToString(), newExtension) };
  }

  /// <summary>
  /// Gets the actual directory of a <see cref="VirtualPath"/>.
  /// </summary>
  public static VirtualPath GetDirectory(this VirtualPath path)
  {
    return path with { Target = Path.GetDirectoryName(path.Target.ToSpan()) };
  }

  /// <summary>
  /// Copies a <see cref="VirtualPath"/> to another <see cref="VirtualPath"/>.
  /// </summary>
  public static async ValueTask CopyToAsync(this VirtualPath from, VirtualPath to, CancellationToken cancellationToken = default)
  {
    await using var input = from.OpenInputStream();
    await using var output = to.OpenOutputStream();

    await input.CopyToAsync(output, cancellationToken);
  }

  /// <summary>
  /// Reads all bytes from a <see cref="VirtualPath"/>.
  /// </summary>
  public static byte[] ReadAllBytes(this VirtualPath path)
  {
    using var stream = path.OpenInputStream();
    using var buffer = new MemoryStream();

    stream.CopyTo(buffer);

    return buffer.ToArray();
  }

  /// <summary>
  /// Asynchronously reads all bytes from a <see cref="VirtualPath"/>.
  /// </summary>
  public static async ValueTask<byte[]> ReadAllBytesAsync(this VirtualPath path, CancellationToken cancellationToken = default)
  {
    await using var stream = path.OpenInputStream();
    await using var buffer = new MemoryStream();

    await stream.CopyToAsync(buffer, cancellationToken);

    return buffer.ToArray();
  }

  /// <summary>
  /// Writes all bytes to a <see cref="VirtualPath"/>.
  /// </summary>
  public static void WriteAllBytes(this VirtualPath path, ReadOnlySpan<byte> data)
  {
    using var stream = path.OpenOutputStream();

    stream.Write(data);
    stream.Flush();
  }

  /// <summary>
  /// Asynchronously writes all bytes to a <see cref="VirtualPath"/>.
  /// </summary>
  public static async ValueTask WriteAllBytesAsync(this VirtualPath path, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
  {
    await using var stream = path.OpenOutputStream();

    await stream.WriteAsync(data, cancellationToken);
    await stream.FlushAsync(cancellationToken);
  }

  /// <summary>
  /// Reads all text from a <see cref="VirtualPath"/>.
  /// </summary>
  public static string ReadAllText(this VirtualPath path)
  {
    return ReadAllText(path, DefaultEncoding);
  }

  /// <summary>
  /// Reads all text from a <see cref="VirtualPath"/>.
  /// </summary>
  public static string ReadAllText(this VirtualPath path, Encoding encoding)
  {
    using var reader = path.OpenInputStreamReader(encoding);

    return reader.ReadToEnd();
  }

  /// <summary>
  /// Asynchronously reads all text from a <see cref="VirtualPath"/>.
  /// </summary>
  public static ValueTask<string> ReadAllTextAsync(this VirtualPath path, CancellationToken cancellationToken = default)
  {
    return ReadAllTextAsync(path, DefaultEncoding, cancellationToken);
  }

  /// <summary>
  /// Asynchronously reads all text from a <see cref="VirtualPath"/>.
  /// </summary>
  public static async ValueTask<string> ReadAllTextAsync(this VirtualPath path, Encoding encoding, CancellationToken cancellationToken = default)
  {
    using var reader = path.OpenInputStreamReader(encoding);

    return await reader.ReadToEndAsync(cancellationToken);
  }

  /// <summary>
  /// Writes all text to a <see cref="VirtualPath"/>.
  /// </summary>
  public static void WriteAllText(this VirtualPath path, string text)
  {
    WriteAllText(path, text, DefaultEncoding);
  }

  /// <summary>
  /// Writes all text to a <see cref="VirtualPath"/>.
  /// </summary>
  public static void WriteAllText(this VirtualPath path, string text, Encoding encoding)
  {
    using var writer = path.OpenOutputStreamWriter(encoding);

    writer.Write(text);
    writer.Flush();
  }

  /// <summary>
  /// Asynchronously writes all text to a <see cref="VirtualPath"/>.
  /// </summary>
  public static ValueTask WriteAllTextAsync(this VirtualPath path, string text, CancellationToken cancellationToken = default)
  {
    return WriteAllTextAsync(path, text, DefaultEncoding, cancellationToken);
  }

  /// <summary>
  /// Asynchronously writes all text to a <see cref="VirtualPath"/>.
  /// </summary>
  public static async ValueTask WriteAllTextAsync(this VirtualPath path, string text, Encoding encoding, CancellationToken cancellationToken = default)
  {
    await using var writer = path.OpenOutputStreamWriter(encoding);

    await writer.WriteAsync(text.AsMemory(), cancellationToken);
    await writer.FlushAsync(cancellationToken);
  }

  /// <summary>
  /// Serializes a value to a <see cref="VirtualPath"/> with the given format.
  /// </summary>
  public static void Serialize<[MeansImplicitUse] T>(this VirtualPath path, T value, FileFormat format)
    where T : class
  {
    using var stream = path.OpenOutputStream();

    format.Serialize(stream, value);
  }

  /// <summary>
  /// Asynchronously serializes a value to a <see cref="VirtualPath"/> with the given format.
  /// </summary>
  public static async ValueTask SerializeAsync<[MeansImplicitUse] T>(this VirtualPath path, T value, FileFormat format, CancellationToken cancellationToken = default)
    where T : class
  {
    await using var stream = path.OpenOutputStream();

    await format.SerializeAsync(stream, value, cancellationToken);
  }

  /// <summary>
  /// Deserializes a value from a <see cref="VirtualPath"/> with the given format.
  /// </summary>
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

  /// <summary>
  /// Asynchronously deserializes a value from a <see cref="VirtualPath"/> with the given format.
  /// </summary>
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

  /// <summary>
  /// Deserializes a value from a <see cref="VirtualPath"/> with the given format.
  /// </summary>
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

  /// <summary>
  /// Asynchronously deserializes a value from a <see cref="VirtualPath"/> with the given format.
  /// </summary>
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
