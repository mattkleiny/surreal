using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.UI;

/// <summary>Context for UI painting operations.</summary>
public interface IPaintingContext
{
  void DrawLine(Vector2 from, Vector2 to, Color color, float thickness = 1);
  void DrawLineStrip(ReadOnlySpan<Vector2> vertices, Color color, float thickness = 1);
  void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Color color);
  void DrawTriangleFan(ReadOnlySpan<Vector2> vertices, Color color);
  void DrawStrokeRect(Rectangle rectangle, Color color);
  void DrawFillRect(Rectangle rectangle, Color color);
  void DrawText(Rectangle rectangle, string text, Color color);
  void DrawTexture(Rectangle rectangle, Texture texture);
}