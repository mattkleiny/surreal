using System.Collections.Generic;
using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Scenes.Entities.Systems {
  internal sealed class SystemManager {
    private readonly EntityScene owner;

    private bool initialized;

    public SystemManager(EntityScene owner) {
      this.owner = owner;
    }

    public List<IEntitySystem> Systems { get; } = new();

    public void Add(IEntitySystem system) {
      Systems.Add(system);

      // initialize the newly added system, if we've already initialized the manager
      if (initialized) {
        system.Initialize(owner);
      }
    }

    public void Initialize() {
      for (var i = 0; i < Systems.Count; i++) {
        var system = Systems[i];

        system.Initialize(owner);
      }

      initialized = true;
    }

    public void Input(DeltaTime deltaTime) {
      for (var i = 0; i < Systems.Count; i++) {
        Systems[i].Input(deltaTime);
      }
    }

    public void Update(DeltaTime deltaTime) {
      for (var i = 0; i < Systems.Count; i++) {
        Systems[i].Update(deltaTime);
      }
    }

    public void Draw(DeltaTime deltaTime) {
      for (var i = 0; i < Systems.Count; i++) {
        Systems[i].Draw(deltaTime);
      }
    }

    public void DisposeAndClear() {
      for (var i = 0; i < Systems.Count; i++) {
        Systems[i].Dispose();
      }

      Systems.Clear();
    }
  }
}