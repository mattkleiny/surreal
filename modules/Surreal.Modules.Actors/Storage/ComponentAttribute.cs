namespace Surreal.Storage;

/// <summary>Adds metadata to a component type.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ComponentAttribute : Attribute
{
	public Type StorageType { get; }

	public ComponentAttribute(Type storageType)
	{
		StorageType = storageType;
	}

	public IComponentStorage<T> CreateStorage<T>()
	{
		var concreteType = StorageType.MakeGenericType(typeof(T));

		return (IComponentStorage<T>) Activator.CreateInstance(concreteType)!;
	}
}
