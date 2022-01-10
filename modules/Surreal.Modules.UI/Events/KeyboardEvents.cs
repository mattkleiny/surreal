using Surreal.Input.Keyboard;

namespace Surreal.UI.Events;

/// <summary>A key was pressed.</summary>
public record KeyDownEvent(Key Key) : IEvent;

/// <summary>A key was released.</summary>
public record KeyUpEvent(Key Key) : IEvent;

/// <summary>A key was pressed and then released.</summary>
public record KeyPressedEvent(Key Key) : IEvent;
