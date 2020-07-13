using System.Collections.Generic;
using Mindustry.Simulation.Modes;
using Surreal.Framework.Scenes.Entities;

namespace Mindustry.Modules {
  public interface IMindustryMod {
    void ConfigureModes(ICollection<GameMode> modes);
    void ConfigureScene(EntityScene scene);
  }
}