using Surreal.UI.Events;

namespace Surreal.UI.Widgets.Retained;

/// <summary>A simple button <see cref="Widget"/> that can be pressed to actuate some event.</summary>
public class Button : Widget
{
	public Content Label { get; set; }
	public Action<Button>? Clicked { get; set; }

	protected internal override void OnComputeLayout(IRetainedModeContext context)
	{
		throw new NotImplementedException();
	}

	protected internal override void OnPaint(IRetainedModeContext context)
	{
		throw new NotImplementedException();
	}

	protected internal override void OnEvent<TEvent>(TEvent e)
	{
		if (e is MouseClickEvent { Button: MouseButton.Left })
		{
			Clicked?.Invoke(this);
		}
	}
}
