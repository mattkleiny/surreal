namespace Surreal.UI.Painting;

/// <summary>A pen is a configured brush used for painting.</summary>
public readonly record struct Pen(Brush Brush, float Thickness = 1f);
