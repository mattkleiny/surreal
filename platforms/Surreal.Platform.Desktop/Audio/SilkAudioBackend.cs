namespace Surreal.Audio;

internal sealed class SilkAudioBackend : IAudioBackend
{
  public IAudioDevice CreateDevice()
  {
    return new SilkAudioDevice();
  }
}
