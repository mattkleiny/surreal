namespace Surreal.Audio;

/// <summary>
/// A <see cref="IAudioBackend"/> implementation that uses Silk.NET.
/// </summary>
internal sealed class SilkAudioBackend : IAudioBackend
{
  public IAudioDevice CreateDevice()
  {
    return new SilkAudioDeviceOpenAL();
  }
}
