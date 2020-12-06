using System.Numerics;
using Surreal.Framework.Scenes.Entities.Aspects;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Scenes.Entities.Systems {
  public sealed class PhysicsSystem : SubscribedSystem {
    private IComponentMapper<Transform>? transforms;
    private IComponentMapper<RigidBody>? rigidBodies;

    public PhysicsSystem()
        : base(Aspect.Of<Transform, RigidBody>()) {
    }

    public float   Friction { get; set; } = 1f;
    public Vector2 Gravity  { get; set; } = new(0f, -9.8f);

    public override void Initialize(EntityScene scene) {
      base.Initialize(scene);

      transforms  = scene.GetComponentMapper<Transform>();
      rigidBodies = scene.GetComponentMapper<RigidBody>();
    }

    public override void Update(DeltaTime deltaTime) {
      ApplyExternalForces(deltaTime);
      ApplyCollisions(deltaTime);
      ApplyInternalForces(deltaTime);
    }

    private void ApplyExternalForces(DeltaTime deltaTime) {
      foreach (var id in Entities) {
        ref var rigidBody = ref rigidBodies!.Get(id);

        // integrate forces
        rigidBody.Acceleration += Gravity * deltaTime;
      }
    }

    private void ApplyCollisions(DeltaTime deltaTime) {
    }

    private void ApplyInternalForces(DeltaTime deltaTime) {
      foreach (var id in Entities) {
        ref var transform = ref transforms!.Get(id);
        ref var rigidBody = ref rigidBodies!.Get(id);

        // integrate forces
        rigidBody.Velocity     += rigidBody.Acceleration / Friction * deltaTime;
        rigidBody.Acceleration += Gravity                           * deltaTime;

        // update position
        transform.Position += rigidBody.Velocity * deltaTime;
      }
    }
  }
}