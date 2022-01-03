using Surreal.Graphics.Textures;

namespace Asteroids.Actors;

public record struct Health(int Amount);
public record struct Sprite(TextureRegion Texture);
