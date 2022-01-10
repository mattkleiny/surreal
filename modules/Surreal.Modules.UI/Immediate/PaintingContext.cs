using Surreal.Mathematics;

namespace Surreal.UI.Immediate;

/// <summary>Phase for immediate mode operations.</summary>
public enum PaintingStage
{
  Layout,
  Paint,
}

/// <summary>Provider for <see cref="PaintingContext"/> control state operations.</summary>
public interface IControlStateProvider
{
  uint       GetControlId();
  ref TState GetControlState<TState>();
}

/// <summary>A context for painting operations in immediate mode.</summary>
public readonly record struct PaintingContext
{
  public PaintingStage          Stage         { get; init; } = PaintingStage.Layout;
  public IControlStateProvider? StateProvider { get; init; } = default;

  public uint GetControlId()
  {
    if (StateProvider == null)
    {
      throw new InvalidOperationException("Unable to access state provider for control ids");
    }

    return StateProvider.GetControlId();
  }

  public ref TState GetControlState<TState>()
  {
    if (StateProvider == null)
    {
      throw new InvalidOperationException("Unable to access state provider for control ids");
    }

    return ref StateProvider.GetControlState<TState>();
  }
}

/// <summary>A layout scope for immediate-mode UI rendering.</summary>
public readonly record struct PaintingLayout(PaintingContext Context)
{
  public Rectangle Rectangle { get; init; } = default;
}
