using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Minecraft.Core.Coordinates;
using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;

namespace Minecraft.Core.Rendering {
  public sealed class WorldRenderer : IDisposable {
    private const string CameraUniform = "u_projView";
    private const string OffsetUniform = "u_offset";

    // TODO: re-use old meshes for new chunks
    private readonly IDictionary<Chunk, ChunkMesh> meshCache = new Dictionary<Chunk, ChunkMesh>();

    private readonly ShaderProgram shader;

    public static async Task<WorldRenderer> CreateAsync(IGraphicsDevice device) {
      var shader = device.Backend.CreateShaderProgram(
          await Shader.LoadAsync(ShaderType.Vertex, "resx://Minecraft.Resources.Shaders.Chunk.vert.glsl"),
          await Shader.LoadAsync(ShaderType.Fragment, "resx://Minecraft.Resources.Shaders.Chunk.frag.glsl")
      );

      return new WorldRenderer(shader);
    }

    private WorldRenderer(ShaderProgram shader) {
      this.shader = shader;
    }

    public Neighborhood Neighborhood { get; set; }

    public void Render(IGraphicsDevice device, World world, ICamera camera, bool wireframe) {
      shader.SetUniform(CameraUniform, in camera.ProjectionView);

      foreach (var chunk in world.GetChunksInNeighborhood(Neighborhood)) {
        if (!meshCache.TryGetValue(chunk, out var mesh)) {
          mesh = meshCache[chunk] = new ChunkMesh(device, chunk);
        }

        // render the mesh, offset based on it's relative world position
        var (x, y, z) = chunk.Position;
        var offset = new Vector3(x * chunk.Width, y * chunk.Height, z * chunk.Depth);

        shader.SetUniform(OffsetUniform, offset);
        mesh.Render(shader, wireframe ? PrimitiveType.Lines : PrimitiveType.Triangles);
      }
    }

    public void Dispose() {
      foreach (var mesh in meshCache.Values) {
        mesh.Dispose();
      }

      shader.Dispose();
    }
  }
}