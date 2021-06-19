namespace Surreal.Framework.Components
{
  public interface IComponentSystem
  {
    void OnInput(GameTime time);
    void OnUpdate(GameTime time);
    void OnDraw(GameTime time);
  }
}