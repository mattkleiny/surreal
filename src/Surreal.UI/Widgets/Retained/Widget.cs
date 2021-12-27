using Surreal.UI.Events;

namespace Surreal.UI.Widgets.Retained;

/// <summary>Base class for any retained mode widget.</summary>
public abstract class Widget
{
	protected internal abstract void OnComputeLayout(IRetainedModeContext context);

	protected internal abstract void OnPaint(IRetainedModeContext context);
	protected internal abstract void OnEvent<TEvent>(TEvent e) where TEvent : IEvent;
}
