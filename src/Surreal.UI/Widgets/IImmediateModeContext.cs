namespace Surreal.UI.Widgets;

/// <summary>The context for on-going immediate-mode (procedural) UI rendering.</summary>
public interface IImmediateModeContext
{
	uint GetControlId();
	ref TState GetControlState<TState>();
}
