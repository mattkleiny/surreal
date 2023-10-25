using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Maths;

namespace Surreal.Scenes;

/// <summary>
/// A node which renders a mesh.
/// </summary>
public class MeshNode3D : SceneNode3D, ICullableObject, IRenderObject
{
  private Mesh? _mesh;
  private Material? _material;

  /// <summary>
  /// The mesh to render.
  /// </summary>
  public Mesh? Mesh
  {
    get => _mesh;
    set => SetField(ref _mesh, value);
  }

  /// <summary>
  /// The material to render the mesh with.
  /// </summary>
  public Material? Material
  {
    get => _material;
    set => SetField(ref _material, value);
  }

  bool ICullableObject.IsVisibleToFrustum(in Frustum frustum)
  {
    return true;
  }

  void IRenderObject.Render(in RenderFrame frame)
  {
    if (_mesh != null && _material != null)
    {
      _mesh.Draw(_material);
    }
  }
}
