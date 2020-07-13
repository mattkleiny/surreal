using Surreal.Memory;

namespace Surreal.Graphics.Meshes {
  public interface IMesh {
    VertexAttributes Attributes { get; }

    int  TriangleCount { get; }
    Size Size          { get; }
  }
}