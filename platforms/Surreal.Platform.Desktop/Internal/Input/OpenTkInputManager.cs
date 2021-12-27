using Surreal.Input;

namespace Surreal.Internal.Input;

internal sealed class OpenTkInputManager : IInputManager
{
	private readonly List<IInputDevice> devices = new();

	public OpenTkInputManager(OpenTkWindow window)
	{
		Keyboard = new OpenTkKeyboardDevice(window);
		Mouse = new OpenTkMouseDevice(window);

		devices.Add(Keyboard);
		devices.Add(Mouse);
	}

	public OpenTkKeyboardDevice Keyboard { get; }
	public OpenTkMouseDevice Mouse { get; }

	public void Update()
	{
		Keyboard.Update();
		Mouse.Update();
	}

	public IEnumerable<IInputDevice> Devices => devices;
}
