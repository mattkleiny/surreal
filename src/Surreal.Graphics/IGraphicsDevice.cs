using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics;

/// <summary>A viewport for scissoring operations on a viewport.</summary>
public readonly record struct Viewport(int X, int Y, int Width, int Height);

/// <summary>Represents the underlying graphics device.</summary>
public interface IGraphicsDevice
{
	IPipelineState Pipeline { get; }

	Viewport Viewport
	{
		get => Pipeline.Rasterizer.Viewport;
		set => Pipeline.Rasterizer.Viewport = value;
	}

	void Clear(Color color);
	void ClearColor(Color color);
	void ClearDepth();

	void DrawMesh<TVertex>(
		Mesh<TVertex> mesh,
		Material material,
		int vertexCount,
		int indexCount,
		PrimitiveType type = PrimitiveType.Triangles
	) where TVertex : unmanaged;

	void BeginFrame();
	void EndFrame();
	void Present();

	GraphicsBuffer<T> CreateBuffer<T>() where T : unmanaged;

	Texture CreateTexture(
		TextureFormat format = TextureFormat.RGBA8888,
		TextureFilterMode filterMode = TextureFilterMode.Linear,
		TextureWrapMode wrapMode = TextureWrapMode.Repeat
	);

	Texture CreateTexture(
		ITextureData data,
		TextureFilterMode filterMode = TextureFilterMode.Linear,
		TextureWrapMode wrapMode = TextureWrapMode.Repeat
	);

	RenderTexture CreateFrameBuffer(in RenderTextureDescriptor descriptor);
}

/// <summary>Represents the underlying state of the graphics fixed-function pipeline.</summary>
public interface IPipelineState
{
	RenderTexture PrimaryRenderTexture { get; }
	RenderTexture? ActiveFrameBuffer { get; set; }
	ShaderProgram? ActiveShader { get; set; }
	GraphicsBuffer? ActiveVertexBuffer { get; set; }
	GraphicsBuffer? ActiveIndexBuffer { get; set; }
	ITextureUnits TextureUnits { get; }
	IRasterizerState Rasterizer { get; }
}

/// <summary>Represents the underlying state of the graphics device rasterizer.</summary>
public interface IRasterizerState
{
	Viewport Viewport { get; set; }

	bool IsDepthTestingEnabled { get; set; }
	bool IsBlendingEnabled { get; set; }
}

/// <summary>Permits interaction with individual texture units on a <see cref="IPipelineState"/>.</summary>
public interface ITextureUnits
{
	Texture? this[int unit] { get; set; }
}
