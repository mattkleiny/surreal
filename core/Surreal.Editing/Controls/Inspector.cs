using Avalonia;
using Avalonia.Controls;

namespace Surreal.Controls;

/// <summary>Allows inspecting an object and settings it's properties</summary>
public sealed class Inspector : ContentControl
{
  public static StyledProperty<object> ObjectProperty { get; } = AvaloniaProperty.Register<Inspector, object>(nameof(Object));

  public object? Object { get; set; }
}

/// <summary>Allows inspecting a single property on an object.</summary>
public sealed class InspectorProperty : Control
{
  public static StyledProperty<string> PathProperty { get; } = AvaloniaProperty.Register<InspectorProperty, string>(nameof(Path));

  public string Path { get; set; } = string.Empty;
}
