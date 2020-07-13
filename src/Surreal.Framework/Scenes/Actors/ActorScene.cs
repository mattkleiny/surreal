using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Surreal.Collections;
using Surreal.Diagnostics.Profiling;
using Surreal.Graphics.Experimental.Rendering.Culling;
using Surreal.Timing;

namespace Surreal.Framework.Scenes.Actors {
  public class ActorScene : IScene, ICullingProvider, IDisposable {
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<ActorScene>();

    public List<Actor> Actors { get; } = new List<Actor>();

    public virtual void Begin() {
      using var _ = Profiler.Track(nameof(Begin));

      for (var i = 0; i < Actors.Count; i++) {
        var actor = Actors[i];
        if (actor.IsEnabled) {
          actor.Begin();
        }
      }
    }

    public virtual void Input(DeltaTime deltaTime) {
      using var _ = Profiler.Track(nameof(Input));

      for (var i = 0; i < Actors.Count; i++) {
        var actor = Actors[i];
        if (actor.IsEnabled) {
          actor.Input(deltaTime);
        }
      }
    }

    public virtual void Update(DeltaTime deltaTime) {
      using var _ = Profiler.Track(nameof(Update));

      for (var i = 0; i < Actors.Count; i++) {
        var actor = Actors[i];
        if (actor.IsEnabled) {
          actor.Update(deltaTime);
        }
      }
    }

    public virtual void Draw(DeltaTime deltaTime) {
      using var _ = Profiler.Track(nameof(Draw));

      for (var i = 0; i < Actors.Count; i++) {
        var actor = Actors[i];
        if (actor.IsEnabled && actor.IsVisible) {
          actor.Draw(deltaTime);
        }
      }
    }

    public virtual void End() {
      using var _ = Profiler.Track(nameof(End));

      for (var i = 0; i < Actors.Count; i++) {
        var actor = Actors[i];
        if (actor.IsEnabled) {
          actor.End();
        }
      }
    }

    public virtual void Dispose() {
      for (var i = 0; i < Actors.Count; i++) {
        Actors[i].Dispose();
      }
    }

    void ICullingProvider.CullRenderers(in CullingViewport viewport, ref SpanList<CulledRenderer> results) {
    }
  }
}