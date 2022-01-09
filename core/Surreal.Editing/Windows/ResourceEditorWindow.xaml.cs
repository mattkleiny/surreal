namespace Surreal.Windows;

/// <summary>A window for editing <see cref="Resource"/>s.</summary>
public partial class ResourceEditorWindow
{
  public ResourceEditorWindow(object resource)
  {
    Resource = resource;

    InitializeComponent();
  }

  public object Resource { get; }
}
