namespace Surreal.Collections.Pooling;

public interface IPoolAware
{
  void OnRent();
  void OnReturn();
}