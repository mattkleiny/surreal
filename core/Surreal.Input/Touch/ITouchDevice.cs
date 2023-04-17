namespace Surreal.Input.Touch;

/// <summary>
/// A touch <see cref="IInputDevice" />.
/// </summary>
public interface ITouchDevice : IInputDevice
{
  ReadOnlySpan<Touch> ActiveTouchPoints { get; }
  event Action<Touch> Touched;
}
