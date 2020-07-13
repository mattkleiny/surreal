using Isaac.Core.Entities.Components;
using Isaac.Core.Events;
using Surreal.Diagnostics.Logging;
using Surreal.Framework.Events;
using Surreal.Framework.Scenes.Entities;
using Surreal.Framework.Scenes.Entities.Aspects;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Framework.Scenes.Entities.Systems;
using Surreal.Timing;

namespace Isaac.Core.Entities.Systems {
  public sealed class CombatSystem : IteratingSystem {
    private static readonly ILog Log = LogFactory.GetLog<CombatSystem>();

    private readonly IEventBus events;

    private IComponentMapper<DamageOverTime> damageOverTimes;

    public CombatSystem(IEventBus events)
        : base(Aspect.Of<Health>()) {
      this.events = events;
    }

    public override void Initialize(EntityScene scene) {
      base.Initialize(scene);

      damageOverTimes = scene.GetComponentMapper<DamageOverTime>();
    }

    protected override void Update(DeltaTime deltaTime, Entity entity) {
      base.Update(deltaTime, entity);

      ApplyDamageOverTime(deltaTime, entity);
    }

    private void ApplyDamageOverTime(DeltaTime deltaTime, Entity entity) {
      if (damageOverTimes.Has(entity.Id)) {
        ref var dot = ref damageOverTimes.Get(entity.Id);

        if (dot.Frequency.Tick(deltaTime)) {
          events.Publish(new DamageEvent(null, entity, dot.Damage));
        }

        if (dot.Lifetime.Tick(deltaTime)) {
          damageOverTimes.Remove(entity.Id);
        }
      }
    }

    [Subscribe]
    public void OnDamage(DamageEvent @event) {
      var source = @event.Source;
      var target = @event.Target;
      var damage = @event.Damage;

      if (target.HasHealth()) {
        ref var health = ref target.Get<Health>();

        health.Apply(damage);

        if (health.Life < 0) {
          events.Publish(new DiedEvent(target));
        }

        Log.Trace(
            source != null
                ? $"{source} damaged {target} for {damage}"
                : $"Unknown damaged {target} for {damage}"
        );
      }
    }

    [Subscribe]
    public void OnDied(DiedEvent @event) {
      var target = @event.Target;

      target.Delete();

      Log.Trace($"{target} has died!");
    }
  }
}