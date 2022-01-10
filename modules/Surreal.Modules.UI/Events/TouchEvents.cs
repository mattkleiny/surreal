using Surreal.Input.Touch;

namespace Surreal.UI.Events;

/// <summary>A touch was pressed.</summary>
public record TouchDownEvent(Touch Touch) : IEvent;

/// <summary>A touch was released.</summary>
public record TouchUpEvent(Touch Touch) : IEvent;
