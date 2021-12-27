using Surreal.Assets;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics.Textures;

/// <summary>Encapsulates a region of a parent <see cref="Texture"/>.</summary>
public readonly record struct TextureRegion(Texture Texture, Vector2I Offset, Vector2I Size) : IDisposable
{
	public int Width => Size.X;
	public int Height => Size.Y;

	public TextureRegion(Texture texture)
		: this(texture, new Vector2I(0, 0), new Vector2I(texture.Width, texture.Height))
	{
	}

	public TextureRegion Slice(Vector2I offset, Vector2I size)
	{
		return new TextureRegion(Texture, Offset + offset, size);
	}

	public void Dispose()
	{
		Texture.Dispose();
	}
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="TextureRegion"/>s.</summary>
public sealed class TextureRegionLoader : AssetLoader<TextureRegion>
{
	public override async Task<TextureRegion> LoadAsync(VirtualPath path, IAssetContext context)
	{
		var texture = await context.LoadAsset<Texture>(path);

		return texture.ToRegion();
	}
}
