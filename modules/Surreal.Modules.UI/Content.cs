namespace Surreal.UI;

/// <summary>Represents content that can be rendered in the UI, containing text an optional tooltip and an optional icon.</summary>
public readonly record struct Content(string? Text, string? ToolTip)
{
  public static implicit operator Content(string text) => new(text, ToolTip: null);
}
