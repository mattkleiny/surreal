using System.Collections.Generic;
using Surreal.Input;

namespace Surreal.Platform.Internal.Input {
  internal sealed class HeadlessInputManager : IInputManager {
    private readonly List<IInputDevice> devices = new List<IInputDevice>();

    public HeadlessInputManager() {
      devices.Add(Keyboard);
      devices.Add(Mouse);
    }

    public IEnumerable<IInputDevice> Devices => devices;

    public HeadlessKeyboardDevice Keyboard { get; } = new HeadlessKeyboardDevice();
    public HeadlessMouseDevice    Mouse    { get; } = new HeadlessMouseDevice();

    public void Update() {
      Keyboard.Update();
      Mouse.Update();
    }
  }
}