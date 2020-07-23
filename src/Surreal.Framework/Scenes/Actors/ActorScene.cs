using System;
using Surreal.Diagnostics.Profiling;
using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Scenes.Actors {
  public sealed class ActorScene : IScene, IDisposable {
    private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<ActorScene>();

    public ActorList Actors { get; } = new ActorList(null);

    public void Input(DeltaTime deltaTime) {
      using var _ = Profiler.Track(nameof(Input));

      for (var i = 0; i < Actors.Count; i++) {
        var actor = Actors[i];
        if (actor.IsEnabled) {
          actor.Input(deltaTime);
        }
      }
    }

    public void Update(DeltaTime deltaTime) {
      using var _ = Profiler.Track(nameof(Update));

      for (var i = 0; i < Actors.Count; i++) {
        var actor = Actors[i];
        if (actor.IsEnabled) {
          actor.Update(deltaTime);
        }
      }
    }

    public void Draw(DeltaTime deltaTime) {
      using var _ = Profiler.Track(nameof(Draw));

      for (var i = 0; i < Actors.Count; i++) {
        var actor = Actors[i];
        if (actor.IsEnabled && actor.IsVisible) {
          actor.Draw(deltaTime);
        }
      }
    }

    public void Dispose() {
      for (var i = 0; i < Actors.Count; i++) {
        Actors[i].Dispose();
      }
    }
  }
}