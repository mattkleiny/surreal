using Surreal.States;

namespace Surreal.Platform.SPI
{
  public abstract class Application : Xamarin.Forms.Application, IApplication
  {
    public FSM<ApplicationState> State { get; } = new FSM<ApplicationState>(ApplicationState.Running);

    protected override void OnStart()  => State.ChangeState(ApplicationState.Running);
    protected override void OnSleep()  => State.ChangeState(ApplicationState.Sleeping);
    protected override void OnResume() => State.ChangeState(ApplicationState.Running);
  }
}
