﻿using Surreal.Mathematics;

namespace Surreal.Input.Touch;

/// <summary>Represents a single touch on a <see cref="ITouchDevice"/>.</summary>
public readonly record struct Touch(uint Id, Vector2I Position, float Pressure)
{
  public override string ToString() => $"Touch at {Position} with {Pressure}";
}