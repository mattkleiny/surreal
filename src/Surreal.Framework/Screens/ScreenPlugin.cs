namespace Surreal.Framework.Screens
{
  public interface IScreenPlugin
  {
    void Show();
    void Hide();

    void Begin();
    void Input(GameTime time);
    void Update(GameTime time);
    void Draw(GameTime time);
    void End();
  }

  public abstract class ScreenPlugin : IScreenPlugin
  {
    public virtual void Show()
    {
    }

    public virtual void Hide()
    {
    }

    public virtual void Begin()
    {
    }

    public virtual void Input(GameTime time)
    {
    }

    public virtual void Update(GameTime time)
    {
    }

    public virtual void Draw(GameTime time)
    {
    }

    public virtual void End()
    {
    }
  }
}
