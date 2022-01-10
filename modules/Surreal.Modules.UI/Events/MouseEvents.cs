using Surreal.Input.Mouse;

namespace Surreal.UI.Events;

/// <summary>A mouse button was pressed.</summary>
public record MouseDownEvent(MouseButton Button) : IEvent;

/// <summary>A mouse button was released.</summary>
public record MouseUpEvent(MouseButton Button) : IEvent;

/// <summary>A mouse button was clicked and then released.</summary>
public record MouseClickEvent(MouseButton Button) : IEvent;
