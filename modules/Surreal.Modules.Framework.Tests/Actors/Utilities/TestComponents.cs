namespace Surreal.Utilities;

internal record struct Transform
{
  public Vector2 Position;
  public float Rotation;
}

internal record struct Sprite
{
  public ConsoleColor BackgroundColor;
  public ConsoleColor ForegroundColor;
  public char Symbol;
}

internal record struct Statistics
{
  public int Health;
  public int Toxin;
}


