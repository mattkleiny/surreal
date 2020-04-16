using Surreal.Framework.Scenes.Entities;

namespace Mindustry.Simulation.Modes
{
  public abstract class GameMode
  {
    public abstract string    Name  { get; }
    public abstract GameRules Rules { get; }

    public abstract void Initialize(EntityScene scene);
  }
}