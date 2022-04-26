﻿using System.Runtime.InteropServices;
using Surreal.Mathematics;

namespace Surreal.Graphics.Meshes;

/// <summary>A 2d vertex type for primitive shapes.</summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Vertex2(Vector2 Position, Color Color, Vector2 UV)
{
  [VertexDescriptor(VertexType.Float, 2)]
  public Vector2 Position = Position;

  [VertexDescriptor(VertexType.Float, 4)]
  public Color Color = Color;

  [VertexDescriptor(VertexType.Float, 2)]
  public Vector2 UV = UV;
}

/// <summary>A 3d vertex type for primitive shapes.</summary>
[StructLayout(LayoutKind.Sequential)]
public record struct Vertex3(Vector3 Position, Color Color, Vector2 UV)
{
  [VertexDescriptor(VertexType.Float, 3)]
  public Vector3 Position = Position;

  [VertexDescriptor(VertexType.Float, 4)]
  public Color Color = Color;

  [VertexDescriptor(VertexType.Float, 2)]
  public Vector2 UV = UV;
}
