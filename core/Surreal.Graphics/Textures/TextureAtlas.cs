using Surreal.Mathematics;

namespace Surreal.Graphics.Textures;

/// <summary>
/// A texture atlas. This is a collection of textures that are packed into a single texture for efficiency.
/// </summary>
public sealed class TextureAtlas(Texture texture, Point2 Size);
