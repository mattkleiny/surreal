using System.Drawing;

namespace Surreal.UI.Widgets.Immediate;

/// <summary>Standard immediate mode widgets for editor and runtime UI construction.</summary>
public static class Widgets
{
	public static void Label(this IImmediateModeContext context, Content label, RectangleF position)
	{
		throw new NotImplementedException();
	}

	public static bool Button(this IImmediateModeContext context, Content label, RectangleF position)
	{
		throw new NotImplementedException();
	}

	public static bool Toggle(this IImmediateModeContext context, RectangleF position, bool enabled, Optional<Content> label = default)
	{
		throw new NotImplementedException();
	}

	public static int VerticalSlider(this IImmediateModeContext context, RectangleF position, int value, Optional<Content> label = default)
	{
		throw new NotImplementedException();
	}

	public static int HorizontalSlider(this IImmediateModeContext context, RectangleF position, int value, Optional<Content> label = default)
	{
		throw new NotImplementedException();
	}
}
