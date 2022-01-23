using Surreal.Assets;
using Surreal.Memory;

namespace Surreal.Audio.Clips;

/// <summary>A clip of audio that can be played back via an audio device.</summary>
public sealed class AudioClip : AudioResource, IHasSizeEstimate
{
  private readonly IAudioServer server;
  private readonly AudioHandle  handle;

  public AudioClip(IAudioServer server)
  {
    this.server = server;
    handle      = server.CreateAudioClip();
  }

  public TimeSpan        Duration   { get; private set; } = TimeSpan.Zero;
  public AudioSampleRate SampleRate { get; private set; } = AudioSampleRate.Standard;
  public Size            Size       { get; private set; } = Size.Zero;

  public void WriteData<T>(TimeSpan duration, AudioSampleRate sampleRate, ReadOnlySpan<T> buffer)
    where T : unmanaged
  {
    Duration   = duration;
    SampleRate = sampleRate;
    Size       = buffer.CalculateSize();

    server.WriteAudioClipData(handle, sampleRate, buffer);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteAudioClip(handle);
    }

    base.Dispose(managed);
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="AudioClip"/>s.</summary>
public sealed class AudioClipLoader : AssetLoader<AudioClip>
{
  private readonly IAudioServer server;

  public AudioClipLoader(IAudioServer server)
  {
    this.server = server;
  }

  public override async ValueTask<AudioClip> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    var buffer = await context.Manager.LoadAssetAsync<AudioBuffer>(context.Path);
    var clip   = new AudioClip(server);

    clip.WriteData<byte>(buffer.Duration, buffer.Rate, buffer.Data.Span);

    return clip;
  }
}
