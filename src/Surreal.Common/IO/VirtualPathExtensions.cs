using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>Static extensions for working with <see cref="VirtualPath"/>s.</summary>
public static class VirtualPathExtensions
{
	private static readonly Encoding DefaultEncoding = Encoding.UTF8;

	public static IFileSystem GetFileSystem(this VirtualPath path)
	{
		return FileSystem.GetForScheme(path.Scheme.ToString()!)!;
	}

	public static VirtualPath Resolve(this VirtualPath path, params string[] name) => path.GetFileSystem().Resolve(path.Target.ToString()!, name);
	public static ValueTask<bool> ExistsAsync(this VirtualPath path) => path.GetFileSystem().ExistsAsync(path.Target.ToString()!);
	public static ValueTask<bool> IsFileAsync(this VirtualPath path) => path.GetFileSystem().IsFileAsync(path.Target.ToString()!);
	public static ValueTask<bool> IsDirectoryAsync(this VirtualPath path) => path.GetFileSystem().IsDirectoryAsync(path.Target.ToString()!);
	public static ValueTask<Size> GetSizeAsync(this VirtualPath path) => path.GetFileSystem().GetSizeAsync(path.Target.ToString()!);

	public static ValueTask<Stream> OpenInputStreamAsync(this VirtualPath path) => path.GetFileSystem().OpenInputStreamAsync(path.Target.ToString()!);
	public static ValueTask<Stream> OpenOutputStreamAsync(this VirtualPath path) => path.GetFileSystem().OpenOutputStreamAsync(path.Target.ToString()!);

	public static IPathWatcher Watch(this VirtualPath path) => path.GetFileSystem().WatchPath(path);

	public static ValueTask<VirtualPath[]> EnumerateAsync(this VirtualPath path, string wildcard)
	{
		return path.GetFileSystem().EnumerateAsync(path.Target.ToString()!, wildcard);
	}

	public static async Task CopyToAsync(this VirtualPath from, VirtualPath to)
	{
		await using var input = await from.OpenInputStreamAsync();
		await using var output = await to.OpenOutputStreamAsync();

		await input.CopyToAsync(output);
	}

	public static async Task<byte[]> ReadAllBytesAsync(this VirtualPath path)
	{
		await using var stream = await path.OpenInputStreamAsync();
		await using var buffer = new MemoryStream();

		await stream.CopyToAsync(buffer);

		return buffer.ToArray();
	}

	public static async Task WriteAllBytesAsync(this VirtualPath path, ReadOnlyMemory<byte> data)
	{
		await using var stream = await path.OpenOutputStreamAsync();

		await stream.WriteAsync(data);
		await stream.FlushAsync();
	}

	public static async Task<string> ReadAllTextAsync(this VirtualPath path, Encoding? encoding = default)
	{
		await using var stream = await path.OpenInputStreamAsync();
		using var reader = new StreamReader(stream, encoding ?? DefaultEncoding);

		return await reader.ReadToEndAsync();
	}

	public static async Task WriteAllTextAsync(this VirtualPath path, string text, Encoding? encoding = default)
	{
		await using var stream = await path.OpenOutputStreamAsync();
		await using var writer = new StreamWriter(stream, encoding ?? DefaultEncoding);

		await writer.WriteAsync(text);
		await writer.FlushAsync();
	}

	public static async Task SerializeBinaryAsync<T>(this VirtualPath path, T value, Optional<Encoding> encoding = default)
		where T : class, IBinarySerializable
	{
		await using var stream = await path.OpenOutputStreamAsync();
		await using var writer = new BinaryWriter(stream, encoding.GetOrDefault(DefaultEncoding));

		value.Serialize(writer);
	}

	public static async Task<T> DeserializeBinaryAsync<T>(this VirtualPath path, Optional<Encoding> encoding = default)
		where T : class, IBinarySerializable, new()
	{
		await using var stream = await path.OpenInputStreamAsync();
		var reader = new BinaryReader(stream, encoding.GetOrDefault(DefaultEncoding));

		var result = new T();
		result.Deserialize(reader);

		return result;
	}

	public static async Task SerializeJsonAsync<T>(this VirtualPath path, T value)
		where T : class
	{
		await using var stream = await path.OpenOutputStreamAsync();

		await JsonSerializer.SerializeAsync(stream, value);
	}

	public static async Task<T?> DeserializeJsonAsync<T>(this VirtualPath path)
		where T : class
	{
		await using var stream = await path.OpenInputStreamAsync();

		return await JsonSerializer.DeserializeAsync<T>(stream);
	}

	public static async Task SerializeXmlAsync<T>(this VirtualPath path, T value)
		where T : class
	{
		await using var stream = await path.OpenOutputStreamAsync();

		var serializer = new XmlSerializer(typeof(T));

		serializer.Serialize(stream, value);
	}

	public static async Task<T> DeserializeXmlAsync<T>(this VirtualPath path)
		where T : class
	{
		await using var stream = await path.OpenInputStreamAsync();
		var serializer = new XmlSerializer(typeof(T));

		return (T) serializer.Deserialize(stream)!;
	}
}
