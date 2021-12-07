using System.Numerics;

namespace Surreal.Mathematics.Linear;

/// <summary>A frustum in 3-space.</summary>
public readonly record struct Frustum(Plane Left, Plane Right, Plane Top, Plane Bottom, Plane Near, Plane Far);