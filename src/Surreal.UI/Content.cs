namespace Surreal.UI;

public readonly record struct Content(string? Text, string? ToolTip)
{
  public static implicit operator Content(string text) => new(text, ToolTip: null);
}