using Surreal.States;
using Xamarin.Forms;

namespace Surreal.Platform.SPI {
  public interface IApplication {
    FSM<ApplicationState> State      { get; }
    IDispatcher           Dispatcher { get; }
  }
}