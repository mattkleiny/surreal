namespace Surreal.Graphics.Textures;

/// <summary>Describes a <see cref="RenderTexture"/>'s underlying texture.</summary>
public readonly record struct RenderTextureDescriptor(int Width, int Height, TextureFormat Format, TextureFilterMode FilterMode);

/// <summary>A frame-buffer that can be used for off-screen rendering.</summary>
public abstract class RenderTexture : GraphicsResource
{
	public abstract Texture Texture { get; }
}
