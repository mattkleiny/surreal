using System.Collections.Generic;
using Mindustry.Modules.Core.Components;
using Mindustry.Modules.Core.Systems;
using Mindustry.Simulation.Modes;
using Surreal;
using Surreal.Framework.Scenes.Entities;
using Surreal.Framework.Scenes.Entities.Storage;

namespace Mindustry.Modules.Core {
  [ExportMod(
      Name        = "Core",
      Description = "The core module for Mindustry",
      Version     = "0.1"
  )]
  public sealed class CoreMod : Mod, IMindustryMod {
    public void ConfigureModes(ICollection<GameMode> modes) {
      modes.Add(new SurvivalMode());
      modes.Add(new SandboxMode());
    }

    public void ConfigureScene(EntityScene scene) {
      scene.RegisterComponent(new DenseComponentStorage<Conveyor>());
      scene.RegisterComponent(new SparseComponentStorage<ResourceNode>());
      scene.RegisterComponent(new SparseComponentStorage<ResourceStore>());

      scene.AddSystem(new ConveyorSystem());
    }

    private sealed class SurvivalMode : GameMode {
      public override string Name { get; } = "Survival";

      public override GameRules Rules { get; } = new GameRules {
          UseWaves     = true,
          UseWaveTimer = true,
          RespawnTime  = 4f
      };

      public override void Initialize(EntityScene scene) {
      }
    }

    private sealed class SandboxMode : GameMode {
      public override string Name { get; } = "Sandbox";

      public override GameRules Rules { get; } = new GameRules {
          UseWaves     = false,
          UseWaveTimer = false,
          RespawnTime  = 0f
      };

      public override void Initialize(EntityScene scene) {
      }
    }
  }
}