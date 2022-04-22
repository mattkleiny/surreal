using Surreal.Audio;
using Surreal.Compute;
using Surreal.Graphics;
using Surreal.Internal.Audio;
using Surreal.Internal.Compute;
using Surreal.Internal.Graphics;
using Surreal.Threading;
using Surreal.Timing;

namespace Surreal;

/// <summary>The <see cref="IPlatformHost"/> for editor environments.</summary>
internal sealed class EditorPlatformHost : IPlatformHost, IServiceModule
{
  private readonly IEditorHostControl hostControl;

  public EditorPlatformHost(IEditorHostControl hostControl)
  {
    this.hostControl = hostControl;

    AudioServer = new OpenTKAudioServer();
    ComputeServer = new OpenTKComputeServer();
    GraphicsServer = new OpenTKGraphicsServer();
  }

  public event Action<int, int> Resized
  {
    add => hostControl.Resized += value;
    remove => hostControl.Resized -= value;
  }

  public OpenTKAudioServer    AudioServer    { get; }
  public OpenTKComputeServer  ComputeServer  { get; }
  public OpenTKGraphicsServer GraphicsServer { get; }

  public int  Width     => hostControl.Width;
  public int  Height    => hostControl.Height;
  public bool IsVisible => hostControl.IsVisible;
  public bool IsFocused => hostControl.IsFocused;
  public bool IsClosing => hostControl.IsClosing;

  public IServiceModule Services   => this;

  public void Tick(DeltaTime deltaTime)
  {
    hostControl.Blit();
  }

  public void Dispose()
  {
  }

  void IServiceModule.RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton(hostControl);
    services.AddSingleton<IAudioServer>(AudioServer);
    services.AddSingleton<IComputeServer>(ComputeServer);
    services.AddSingleton<IGraphicsServer>(GraphicsServer);
  }
}
