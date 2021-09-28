using System;

namespace Surreal.Input.Touch
{
  /// <summary>A touch <see cref="IInputDevice"/>.</summary>
  public interface ITouchDevice : IInputDevice
  {
    event Action<Touch> Touched;

    ReadOnlySpan<Touch> ActiveTouchPoints { get; }
  }
}
