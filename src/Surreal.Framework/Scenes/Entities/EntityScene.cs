using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Surreal.Collections;
using Surreal.Diagnostics.Profiling;
using Surreal.Framework.Scenes.Entities.Aspects;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Framework.Scenes.Entities.Storage;
using Surreal.Framework.Scenes.Entities.Systems;
using Surreal.Graphics.Rendering.Culling;
using Surreal.Timing;

namespace Surreal.Framework.Scenes.Entities {
  public sealed class EntityScene : IScene, ICullingProvider, IDisposable {
    private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<EntityScene>();

    public EntityScene()
        : this(maxEntityCount: 1_000, maxComponentTypes: 32) {
    }

    public EntityScene(int maxEntityCount, int maxComponentTypes) {
      ComponentManager = new ComponentManager(maxComponentTypes);
      EntityManager    = new EntityManager(maxEntityCount);
      AspectManager    = new AspectManager();
      SystemManager    = new SystemManager(this);
    }

    public bool CompactOnDispose { get; set; } = true;

    internal AspectManager    AspectManager    { get; }
    internal ComponentManager ComponentManager { get; }
    internal EntityManager    EntityManager    { get; }
    internal SystemManager    SystemManager    { get; }

    public event Action<IEntitySystem>  SystemAdded;
    public IReadOnlyList<IEntitySystem> Systems => SystemManager.Systems;

    public void RegisterComponent<T>(IComponentStorage<T> storage)
        where T : IComponent {
      ComponentManager.RegisterComponent(storage);
    }

    public void AddSystem(IEntitySystem system) {
      SystemManager.Add(system);
      SystemAdded?.Invoke(system);
    }

    public Entity GetEntity(EntityId id) {
      return new Entity(id, this);
    }

    public Entity CreateEntity() {
      return GetEntity(EntityManager.Create());
    }

    public void DeleteEntity(EntityId id) {
      EntityManager.Remove(id);
    }

    public TSystem? GetSystem<TSystem>()
        where TSystem : class, IEntitySystem {
      return SystemManager.Systems.OfType<TSystem>().FirstOrDefault();
    }

    public IComponentMapper<T> GetComponentMapper<T>()
        where T : IComponent, new() {
      return ComponentManager.GetComponentMapper<T>();
    }

    public IAspectSubscription Subscribe(Aspect aspect) {
      return AspectManager.Subscribe(aspect);
    }

    public void Initialize() {
      using var _ = Profiler.Track(nameof(Initialize));

      SystemManager.Initialize();
    }

    public void Begin() {
      using var _ = Profiler.Track(nameof(Begin));

      SystemManager.Begin();
    }

    public void Input(DeltaTime deltaTime) {
      using var _ = Profiler.Track(nameof(Input));

      SystemManager.Input(deltaTime);
    }

    public void Update(DeltaTime deltaTime) {
      using var _ = Profiler.Track(nameof(Update));

      SystemManager.Update(deltaTime);
    }

    public void Draw(DeltaTime deltaTime) {
      using var _ = Profiler.Track(nameof(Draw));

      SystemManager.Draw(deltaTime);
    }

    public void End() {
      using var _ = Profiler.Track(nameof(End));

      SystemManager.End();

      if (EntityManager.Flush(out var addedEntities, out var removedEntities)) {
        AspectManager.Refresh(addedEntities, removedEntities);
        ComponentManager.Cull(removedEntities);
      }
    }

    public void Dispose() {
      SystemManager.DisposeAndClear();
      ComponentManager.DisposeAndClear();

      // entity data is usually dense and occupies the LOH; ask the GC to perform a collection to reduce the
      // long-term running costs of old entity data.
      if (CompactOnDispose) {
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect();
      }
    }

    void ICullingProvider.CullRenderers(in CullingViewport viewport, ref SpanList<CulledRenderer> results) {
    }
  }
}