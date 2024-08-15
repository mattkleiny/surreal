using Silk.NET.Input;
using Surreal.Input.Gamepad;

namespace Surreal.Input;

/// <summary>
/// A <see cref="IGamepadDevice"/> implementation that uses Silk.NET.
/// </summary>
internal sealed class SilkGamepadDevice(IGamepad gamepad) : IGamepadDevice;
