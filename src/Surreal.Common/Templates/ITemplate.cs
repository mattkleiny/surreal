namespace Surreal.Templates;

/// <summary>A template for objects of type <see cref="T"/>.</summary>
public interface ITemplate
{
	object Create();
}

/// <summary>A strongly-typed <see cref="ITemplate"/></summary>
public interface ITemplate<out T> : ITemplate
{
	new T Create();

	object ITemplate.Create()
	{
		return Create()!;
	}
}
