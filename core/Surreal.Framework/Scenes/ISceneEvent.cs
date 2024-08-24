using Surreal.Timing;

namespace Surreal.Scenes;

/// <summary>
/// Represents an event that can be sent to a scene.
/// </summary>
public interface ISceneEvent;

/// <summary>
/// An event that is sent to a scene every frame to update the scene.
/// </summary>
public readonly record struct TickEvent(DeltaTime DeltaTime) : ISceneEvent;
