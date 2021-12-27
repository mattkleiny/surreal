using Surreal.Aspects;

namespace Surreal.Systems;

/// <summary>Context for component system operations.</summary>
public interface IComponentSystemContext
{
	/// <summary>Queries actors that match a given <see cref="Aspect"/>.</summary>
	AspectEnumerator QueryActors(Aspect aspect);
}
