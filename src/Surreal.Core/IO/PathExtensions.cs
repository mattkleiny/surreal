using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Surreal.IO.VFS;
using Utf8Json;

namespace Surreal.IO {
  public static class PathExtensions {
    private const int DefaultBufferSize = 4096;

    private static readonly BinaryFormatter Formatter = new BinaryFormatter();

    public static IFileSystem GetFileSystem(this Path path) => FileSystems.GetForScheme(path.Scheme)!;

    public static string GetExtension(this Path path) => System.IO.Path.GetExtension(path.Target);

    public static async Task<bool> ExistsAsync(this Path path)      => await path.GetFileSystem().ExistsAsync(path.Target);
    public static async Task<bool> IsFileAsync(this Path path)      => await path.GetFileSystem().IsFileAsync(path.Target);
    public static async Task<bool> IsDirectoryAsync(this Path path) => await path.GetFileSystem().IsDirectoryAsync(path.Target);

    public static Path Resolve(this Path path, string name) {
      return path.GetFileSystem().Resolve(path.Target, name);
    }

    public static Task<Path[]> EnumerateAsync(this Path path, string wildcard) {
      return path.GetFileSystem().EnumerateAsync(path.Target, wildcard);
    }

    public static async Task<Size> GetSizeAsync(this Path path) {
      return await path.GetFileSystem().GetSizeAsync(path.Target);
    }

    public static async Task<T> DeserializeBinaryAsync<T>(this Path path) {
      await using var stream = await path.OpenInputStreamAsync();

      return (T) Formatter.Deserialize(stream);
    }

    public static async Task<T> DeserializeJsonAsync<T>(this Path path) {
      await using var stream = await path.OpenInputStreamAsync();

      return JsonSerializer.Deserialize<T>(stream);
    }

    public static async Task<T> DeserializeXmlAsync<T>(this Path path) {
      await using var stream = await path.OpenInputStreamAsync();

      var serializer = new XmlSerializer(typeof(T));

      return (T) serializer.Deserialize(stream)!;
    }

    public static async Task SerializeBinaryAsync<T>(this Path path, T value) {
      await using var stream = await path.OpenOutputStreamAsync();

      Formatter.Serialize(stream, value!);
    }

    public static async Task SerializeJsonAsync<T>(this Path path, T value) {
      await using var stream = await path.OpenOutputStreamAsync();

      JsonSerializer.Serialize(stream, value);
    }

    public static async Task SerializeXmlAsync<T>(this Path path, T value) {
      await using var stream = await path.OpenOutputStreamAsync();

      var serializer = new XmlSerializer(typeof(T));

      serializer.Serialize(stream, value);
    }

    public static async Task<Stream> OpenInputStreamAsync(this Path path, int bufferSize = DefaultBufferSize) {
      var stream = await path.GetFileSystem().OpenInputStreamAsync(path.Target);

      if (bufferSize > 0) {
        return new BufferedStream(stream, bufferSize);
      }

      return stream;
    }

    public static async Task<Stream> OpenOutputStreamAsync(this Path path, int bufferSize = DefaultBufferSize) {
      var stream = await path.GetFileSystem().OpenOutputStreamAsync(path.Target);

      if (bufferSize > 0) {
        return new BufferedStream(stream, bufferSize);
      }

      return stream;
    }

    public static async Task CopyToAsync(this Path from, Path to, int bufferSize = DefaultBufferSize) {
      await using var input  = await from.OpenInputStreamAsync(bufferSize);
      await using var output = await to.OpenOutputStreamAsync(bufferSize);

      await input.CopyToAsync(output);
    }

    public static IPathWatcher Watch(this Path path) {
      var fileSystem = path.GetFileSystem();

      if (!fileSystem.SupportsWatcher) {
        throw new InvalidOperationException($"The path '{path}' does not support path watchers.");
      }

      return fileSystem.WatchPath(path);
    }

    public static async Task<byte[]> ReadAllBytesAsync(this Path path, int bufferSize = DefaultBufferSize) {
      await using var stream = await path.OpenInputStreamAsync(bufferSize);
      await using var buffer = new MemoryStream();

      await stream.CopyToAsync(buffer);

      return buffer.ToArray();
    }

    public static Task<string> ReadAllTextAsync(this Path path, int bufferSize = DefaultBufferSize) {
      return ReadAllTextAsync(path, Encoding.UTF8, bufferSize);
    }

    public static async Task<string> ReadAllTextAsync(this Path path, Encoding encoding, int bufferSize = DefaultBufferSize) {
      await using var stream = await path.OpenInputStreamAsync(bufferSize);
      using var       reader = new StreamReader(stream, encoding);

      return await reader.ReadToEndAsync();
    }
  }
}