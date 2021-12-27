namespace Surreal.Collections.Pooling;

/// <summary>Permits an object to respond to pool callbacks.</summary>
public interface IPoolAware
{
	void OnRent();
	void OnReturn();
}
