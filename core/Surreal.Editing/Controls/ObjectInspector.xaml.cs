using Surreal.Objects;

namespace Surreal.Controls;

/// <summary>An inspector for objects.</summary>
public partial class ObjectInspector
{
  public ObjectInspector()
  {
    InitializeComponent();
  }

  public object         Target   { get; }
  public ObjectMetadata Metadata { get; }
}
