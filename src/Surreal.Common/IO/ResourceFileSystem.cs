using System.Reflection;
using Surreal.Memory;

namespace Surreal.IO;

/// <summary>A <see cref="FileSystem"/> that uses embed assembly resources.</summary>
public sealed class ResourceFileSystem : FileSystem
{
	private static Assembly[] GetDefaultAssemblies()
	{
		return AppDomain.CurrentDomain
			.GetAssemblies()
			// dynamic assemblies do not support resources
			.Where(assembly => !assembly.IsDynamic)
			.ToArray();
	}

	private readonly Assembly[] assemblies;

	public ResourceFileSystem()
		: this(GetDefaultAssemblies())
	{
	}

	public ResourceFileSystem(params Assembly[] assemblies)
		: base("resource", "resources", "embedded", "resx")
	{
		this.assemblies = assemblies;
	}

	public override VirtualPath Resolve(string root, params string[] paths) => string.Join(root, ".", string.Join(".", paths));

	public override ValueTask<VirtualPath[]> EnumerateAsync(string path, string wildcard) => throw new NotSupportedException();

	public override ValueTask<Size> GetSizeAsync(string path) => throw new NotSupportedException();
	public override ValueTask<bool> IsFileAsync(string path) => throw new NotSupportedException();
	public override ValueTask<bool> IsDirectoryAsync(string path) => throw new NotSupportedException();
	public override ValueTask<bool> ExistsAsync(string path) => throw new NotSupportedException();

	public override ValueTask<Stream> OpenInputStreamAsync(string path)
	{
		foreach (var assembly in assemblies)
		{
			var stream = assembly.GetManifestResourceStream(NormalizePath(path));

			if (stream != null)
			{
				return ValueTask.FromResult(stream);
			}
		}

		throw new ResourceNotFoundException("Unable to locate resource: " + path);
	}

	public override ValueTask<Stream> OpenOutputStreamAsync(string path) => throw new NotSupportedException();

	private static string NormalizePath(string path)
	{
		return path.Replace('/', '.');
	}
}

public sealed class ResourceNotFoundException : Exception
{
	public ResourceNotFoundException(string message)
		: base(message)
	{
	}
}
