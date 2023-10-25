namespace Surreal.Diagnostics.Gizmos;

/// <summary>
/// Represents a kind of object that can render it's own gizmos.
/// </summary>
public interface IGizmoObject
{
  /// <summary>
  /// Renders the gizmos for this object.
  /// </summary>
  void RenderGizmos(IGizmoBatch gizmos);
}
