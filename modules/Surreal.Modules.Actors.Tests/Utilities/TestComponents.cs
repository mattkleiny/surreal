﻿namespace Surreal.Utilities;

internal record struct Transform
{
  public Vector2 Position;
  public float Rotation;
}

internal record struct Sprite
{
  public char Symbol;
  public ConsoleColor ForegroundColor;
  public ConsoleColor BackgroundColor;
}

internal record struct Statistics
{
  public int Health;
  public int Toxin;
}