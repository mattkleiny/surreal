using Surreal.Memory;

namespace Surreal.IO;

/// <summary>A <see cref="FileSystem"/> that uses a <see cref="IFilePackingScheme"/>.</summary>
public sealed class PackedFileSystem : FileSystem
{
	private readonly IFilePackingScheme scheme;

	public PackedFileSystem(IFilePackingScheme scheme)
		: base("pak")
	{
		this.scheme = scheme;
	}

	public override VirtualPath Resolve(string root, params string[] paths)
	{
		throw new NotImplementedException();
	}

	public override ValueTask<VirtualPath[]> EnumerateAsync(string path, string wildcard)
	{
		throw new NotImplementedException();
	}

	public override ValueTask<Size> GetSizeAsync(string path)
	{
		throw new NotImplementedException();
	}

	public override ValueTask<bool> IsDirectoryAsync(string path)
	{
		throw new NotImplementedException();
	}

	public override ValueTask<bool> ExistsAsync(string path)
	{
		throw new NotImplementedException();
	}

	public override ValueTask<bool> IsFileAsync(string path)
	{
		throw new NotImplementedException();
	}

	public override ValueTask<Stream> OpenInputStreamAsync(string path)
	{
		throw new NotImplementedException();
	}

	public override ValueTask<Stream> OpenOutputStreamAsync(string path)
	{
		throw new NotImplementedException();
	}
}
