using Surreal.Input.Keyboard;

namespace Surreal.Platform
{
  public interface IHeadlessKeyboardDevice : IKeyboardDevice
  {
    bool this[Key key] { get; set; }
  }
}