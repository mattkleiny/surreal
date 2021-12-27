namespace Surreal.Collections;

/// <summary>Identifies a single property in a <see cref="IBlackboard"/>, with a default value.</summary>
public readonly record struct BlackboardProperty<T>(string Key, T DefaultValue = default!)
{
	public override int GetHashCode()
	{
		return string.GetHashCode(Key, StringComparison.OrdinalIgnoreCase);
	}

	public bool Equals(BlackboardProperty<T> other)
	{
		return string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase);
	}
}

/// <summary>A blackboard is a dictionary of structured types.</summary>
public interface IBlackboard
{
	T Get<T>(BlackboardProperty<T> property, Optional<T> defaultValue = default);
	void Set<T>(BlackboardProperty<T> property, T value);
	void Remove<T>(BlackboardProperty<T> property);
	void Clear();
}

/// <summary>The default <see cref="IBlackboard"/> implementation.</summary>
public sealed class Blackboard : IBlackboard
{
	private readonly Dictionary<Type, object> storageByKey = new();

	public T Get<T>(BlackboardProperty<T> property, Optional<T> defaultValue = default)
	{
		if (TryGetStorage<T>(out var storage) && storage.TryGetValue(property.Key, out var value))
		{
			return value;
		}

		return defaultValue.GetOrDefault(property.DefaultValue);
	}

	public void Set<T>(BlackboardProperty<T> property, T value)
	{
		var storage = GetOrCreateStorage<T>();

		storage[property.Key] = value;
	}

	public void Remove<T>(BlackboardProperty<T> property)
	{
		if (TryGetStorage<T>(out var storage))
		{
			storage.Remove(property.Key);
		}
	}

	public void Clear()
	{
		storageByKey.Clear();
	}

	private Dictionary<string, T> GetOrCreateStorage<T>()
	{
		if (!TryGetStorage<T>(out var storage))
		{
			storageByKey[typeof(T)] = storage = new Dictionary<string, T>(0, StringComparer.OrdinalIgnoreCase);
		}

		return storage;
	}

	private bool TryGetStorage<T>(out Dictionary<string, T> result)
	{
		if (storageByKey.TryGetValue(typeof(T), out var dictionary))
		{
			result = (Dictionary<string, T>) dictionary;
			return true;
		}

		result = default!;
		return false;
	}
}
