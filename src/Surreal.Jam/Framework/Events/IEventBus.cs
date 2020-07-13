namespace Surreal.Framework.Events {
  public interface IEventBus {
    void RegisterListeners(object target);
    void Publish(object @event);
  }
}