using Surreal.Mathematics;

namespace Surreal.Diagnostics.Gizmos;

/// <summary>A context for rendering gizmos and help indicators during editing/etc.</summary>
public interface IGizmoContext
{
  // 2d primitives
  void DrawCircle(Vector2 center, float radius);
  void DrawRect(Vector2 center, Vector2 size);
  void DrawRect(Rectangle rectangle);

  // 3d primitives
  void DrawLine(Vector3 start, Vector3 end);
  void DrawSphere(Vector3 center, float radius);
  void DrawWireBox(Vector3 center, Vector3 size);
  void DrawSolidBox(Vector3 center, Vector3 size);
}
