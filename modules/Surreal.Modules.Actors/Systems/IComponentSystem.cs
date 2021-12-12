using Surreal.Timing;

namespace Surreal.Systems;

/// <summary>Represents a component system, capable of operating on components.</summary>
public interface IComponentSystem
{
  void OnInput(DeltaTime time);
  void OnUpdate(DeltaTime time);
  void OnDraw(DeltaTime time);
}