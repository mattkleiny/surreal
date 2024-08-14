using Surreal.Mathematics;

namespace Surreal.Graphics.Textures;

/// <summary>
/// A texture atlas.
/// <para/>
/// This is a collection of textures that are packed into a single texture for efficiency.
/// <para/>
/// Methods are provided to simplify both creation and querying of the atlas from the CPU,
/// though GPU-side querying is also possible with the appropriate shader code.
/// </summary>
public sealed class TextureAtlas(Texture texture, Point2 Size);
