using System;

namespace Surreal.Input.Touch {
  public interface ITouchDevice : IInputDevice {
    event Action<Touch> Touched;

    ReadOnlySpan<Touch> ActiveTouchPoints { get; }
  }
}