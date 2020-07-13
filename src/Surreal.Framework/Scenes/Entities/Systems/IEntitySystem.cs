using System;
using Surreal.Timing;

namespace Surreal.Framework.Scenes.Entities.Systems {
  public interface IEntitySystem : IDisposable {
    void Initialize(EntityScene scene);

    void Input(DeltaTime deltaTime);
    void Update(DeltaTime deltaTime);
    void Draw(DeltaTime deltaTime);
  }
}