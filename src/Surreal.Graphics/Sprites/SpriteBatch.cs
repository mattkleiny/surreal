using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Surreal.Graphics.Materials.Shaders;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics.Sprites {
  // TODO: support multiple textures in the sprite batch simultaneously?

  public sealed class SpriteBatch : IDisposable {
    private const int    MaximumSpriteCount = 8000;
    private const string TextureUniform     = "u_texture";
    private const string ProjViewUniform    = "u_projView";

    private readonly IDisposableBuffer<Vertex> vertices;
    private readonly Mesh<Vertex>              mesh;
    private readonly ShaderProgram             defaultShader;
    private readonly bool                      ownsDefaultShader;

    private Texture? lastTexture;
    private int      vertexCount;

    public static SpriteBatch Create(IGraphicsDevice device, ShaderProgram shader, int spriteCount = 1000) {
      Debug.Assert(spriteCount > 0, "spriteCount > 0");

      return new SpriteBatch(device, spriteCount, shader, ownsDefaultShader: false);
    }

    public static async Task<SpriteBatch> CreateDefaultAsync(IGraphicsDevice device, int spriteCount = 1000) {
      Debug.Assert(spriteCount > 0, "spriteCount > 0");

      var shader = device.CreateShaderProgram(
          await Shader.LoadAsync(ShaderType.Vertex, "resx://Surreal.Graphics/Resources/Shaders/SpriteBatch.vert.glsl"),
          await Shader.LoadAsync(ShaderType.Fragment, "resx://Surreal.Graphics/Resources/Shaders/SpriteBatch.frag.glsl")
      );

      return new SpriteBatch(device, spriteCount, shader, ownsDefaultShader: true);
    }

    private SpriteBatch(IGraphicsDevice device, int spriteCount, ShaderProgram defaultShader, bool ownsDefaultShader) {
      Debug.Assert(spriteCount > 0, "spriteCount > 0");
      Debug.Assert(spriteCount < MaximumSpriteCount, "spriteCount < MaximumSpriteCount");

      Device             = device;
      MaximumVertexCount = spriteCount * 4;

      this.defaultShader     = defaultShader;
      this.ownsDefaultShader = ownsDefaultShader;

      vertices = Buffers.AllocateOffHeap<Vertex>(MaximumVertexCount);
      mesh     = new Mesh<Vertex>(device);

      CreateIndices(spriteCount * 6); // sprites are simple quads; we can create the indices up-front
    }

    public Color           Color              { get; set; } = Color.White;
    public IGraphicsDevice Device             { get; }
    public ShaderProgram?  ActiveShader       { get; private set; }
    public int             MaximumVertexCount { get; }

    public void Begin(in Matrix4x4 projectionView) {
      Begin(defaultShader, in projectionView);
    }

    public void Begin(ShaderProgram shader, in Matrix4x4 projectionView) {
      shader.SetUniform(TextureUniform, 0);
      shader.SetUniform(ProjViewUniform, in projectionView);

      ActiveShader = shader;
    }

    public void Draw(Texture texture, float x, float y, Angle rotation, float width, float height)
      => DrawInternal(texture, x, y, width, height, rotation, 0, 0, texture.Width, texture.Height, Color);

    public void Draw(TextureRegion region, float x, float y, Angle rotation, float width, float height)
      => DrawInternal(region.Texture, x, y, width, height, rotation, region.OffsetX, region.OffsetY, region.Width, region.Height, Color);

    private void DrawInternal(
        Texture texture,
        float x, float y,
        float width, float height,
        Angle rotation,
        float sourceX, float sourceY,
        float sourceWidth, float sourceHeight,
        Color color) {
      // if we're switching texture, we'll need to flush and start again
      if (texture != lastTexture) {
        Flush();
        lastTexture = texture;
      }
      // if we've exceeded the batch capacity, we'll need to flush and start again
      else if (vertexCount >= MaximumVertexCount) {
        Flush();
      }

      // calculate u/v extents
      var u  = sourceX / texture.Width;
      var v  = (sourceY + sourceHeight) / texture.Height;
      var u2 = (sourceX + sourceWidth) / texture.Width;
      var v2 = sourceY / texture.Height;

      // calculate shape extents
      var extentX = x + width;
      var extentY = y + height;

      // rotate coordinates about the z axis
      if (MathF.Abs(rotation.Radians) > float.Epsilon) {
        throw new NotImplementedException();
      }

      // add vertex data to our batch
      var vertices = this.vertices.Span[vertexCount..];

      ref var vertex0 = ref vertices[0];
      ref var vertex1 = ref vertices[1];
      ref var vertex2 = ref vertices[2];
      ref var vertex3 = ref vertices[3];

      vertex0.Position.X = x;
      vertex0.Position.Y = y;
      vertex0.Color      = color;
      vertex0.UV.X       = u;
      vertex0.UV.Y       = v;

      vertex1.Position.X = x;
      vertex1.Position.Y = extentY;
      vertex1.Color      = color;
      vertex1.UV.X       = u;
      vertex1.UV.Y       = v2;

      vertex2.Position.X = extentX;
      vertex2.Position.Y = extentY;
      vertex2.Color      = color;
      vertex2.UV.X       = u2;
      vertex2.UV.Y       = v2;

      vertex3.Position.X = extentX;
      vertex3.Position.Y = y;
      vertex3.Color      = color;
      vertex3.UV.X       = u2;
      vertex3.UV.Y       = v;

      vertexCount += 4;
    }

    public void End() {
      Flush();
    }

    public void Flush() {
      if (vertexCount == 0) return; // no vertices? don't render

      var spriteCount = vertexCount / 4;
      var indexCount  = spriteCount * 6;

      Device.Pipeline.TextureUnits[0] = lastTexture;

      mesh.Vertices.Write(vertices.Span[..indexCount]);
      mesh.DrawImmediate(ActiveShader!, vertexCount, indexCount);

      vertexCount = 0;
    }

    private void CreateIndices(int indexCount) {
      Span<ushort> indices = stackalloc ushort[indexCount];

      for (ushort i = 0, j = 0; i < indexCount; i += 6, j += 4) {
        indices[i + 0] = j;
        indices[i + 1] = (ushort) (j + 1);
        indices[i + 2] = (ushort) (j + 2);
        indices[i + 3] = (ushort) (j + 2);
        indices[i + 4] = (ushort) (j + 3);
        indices[i + 5] = j;
      }

      mesh.Indices.Write(indices);
    }

    public void Dispose() {
      if (ownsDefaultShader) {
        defaultShader.Dispose();
      }

      mesh.Dispose();
      vertices.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Vertex {
      [VertexAttribute(
          Alias = "a_position",
          Count = 2,
          Type  = VertexType.Float
      )]
      public Vector2 Position;

      [VertexAttribute(
          Alias      = "a_color",
          Count      = 4,
          Type       = VertexType.UnsignedByte,
          Normalized = true
      )]
      public Color Color;

      [VertexAttribute(
          Alias = "a_texCoords",
          Count = 2,
          Type  = VertexType.Float
      )]
      public Vector2 UV;

      public Vertex(Vector2 position, Color color, Vector2 uv) {
        Position = position;
        Color    = color;
        UV       = uv;
      }
    }
  }
}