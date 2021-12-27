namespace Surreal.IO;

/// <summary>Allows watching <see cref="Path"/>s for changes.</summary>
public interface IPathWatcher : IDisposable
{
	VirtualPath Path { get; }

	event Action<VirtualPath> Created;
	event Action<VirtualPath> Modified;
	event Action<VirtualPath> Deleted;
}
