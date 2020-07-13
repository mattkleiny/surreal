using System.Threading.Tasks;
using Surreal.Framework.Scenes.Entities.Aspects;
using Surreal.Mathematics.Timing;

namespace Surreal.Framework.Scenes.Entities.Systems {
  public abstract class ParallelIteratingSystem : IteratingSystem {
    private readonly ParallelOptions options;

    protected ParallelIteratingSystem(Aspect aspect, TaskScheduler? scheduler = null)
        : base(aspect) {
      options = new ParallelOptions {
          TaskScheduler = scheduler ?? TaskScheduler.Default
      };
    }

    public override void Update(DeltaTime deltaTime) {
      var cached = deltaTime.TimeSpan;

      Parallel.For(0, Entities.Length, options, index => {
        var entityId = Entities[index];
        var entity   = World!.GetEntity(entityId);

        Update(cached, entity);
      });
    }
  }
}