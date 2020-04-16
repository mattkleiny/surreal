namespace Surreal.Audio.SPI
{
  public interface IAudioBackend
  {
    IAudioFactory Factory { get; }

    float MasterVolume { get; set; }
  }
}
