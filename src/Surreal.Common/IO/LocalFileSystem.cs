using Surreal.Memory;

namespace Surreal.IO;

/// <summary>A <see cref="FileSystem"/> for the host operating system.</summary>
public sealed class LocalFileSystem : FileSystem
{
	private static readonly string PathSeparator = Path.PathSeparator.ToString();

	public LocalFileSystem()
		: base("local")
	{
	}

	public override VirtualPath Resolve(string root, params string[] paths)
	{
		return string.Join(root, PathSeparator, string.Join(PathSeparator, paths));
	}

	public override bool SupportsWatcher => true;

	public override ValueTask<VirtualPath[]> EnumerateAsync(string path, string wildcard)
	{
		var files = Directory
			.GetFiles(path, wildcard, SearchOption.AllDirectories)
			.Select(_ => new VirtualPath("local", _))
			.ToArray();

		return ValueTask.FromResult(files);
	}

	public override ValueTask<Size> GetSizeAsync(string path)
	{
		return ValueTask.FromResult(new Size(new FileInfo(path).Length));
	}

	public override async ValueTask<bool> ExistsAsync(string path)
	{
		return await IsDirectoryAsync(path) || await IsFileAsync(path);
	}

	public override ValueTask<bool> IsDirectoryAsync(string path)
	{
		return ValueTask.FromResult(Directory.Exists(path));
	}

	public override ValueTask<bool> IsFileAsync(string path)
	{
		return ValueTask.FromResult(File.Exists(path));
	}

	public override ValueTask<Stream> OpenInputStreamAsync(string path)
	{
		return ValueTask.FromResult<Stream>(File.Open(path, FileMode.Open));
	}

	public override ValueTask<Stream> OpenOutputStreamAsync(string path)
	{
		return ValueTask.FromResult<Stream>(File.Open(path, FileMode.OpenOrCreate));
	}

	public override IPathWatcher WatchPath(VirtualPath path)
	{
		return new PathWatcher(path);
	}

	private sealed class PathWatcher : IPathWatcher
	{
		private readonly FileSystemWatcher watcher;

		public VirtualPath Path { get; }

		public event Action<VirtualPath>? Created;
		public event Action<VirtualPath>? Modified;
		public event Action<VirtualPath>? Deleted;

		public PathWatcher(VirtualPath path)
		{
			watcher = new FileSystemWatcher(path.Target.ToString()!);

			// adapt the event interface
			watcher.Created += (_, _) => Created?.Invoke(path);
			watcher.Changed += (_, _) => Modified?.Invoke(path);
			watcher.Renamed += (_, _) => Modified?.Invoke(path);
			watcher.Deleted += (_, _) => Deleted?.Invoke(path);

			Path = path;
		}

		public void Dispose()
		{
			watcher.Dispose();
		}
	}
}
