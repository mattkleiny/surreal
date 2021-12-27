using Surreal.Input.Keyboard;

namespace Surreal;

/// <summary>Allows access to the headless <see cref="IKeyboardDevice"/>.</summary>
public interface IHeadlessKeyboardDevice : IKeyboardDevice
{
	bool this[Key key] { get; set; }
}
