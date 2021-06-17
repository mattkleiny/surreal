using System;
using Surreal.Assets;
using Surreal.Data;

namespace Surreal.Audio.Clips {
  public abstract class AudioBuffer : AudioResource, IAudioData, IHasSizeEstimate {
    public abstract TimeSpan        Duration { get; }
    public abstract AudioSampleRate Rate     { get; }
    public abstract Size            Size     { get; }
    public abstract Span<byte>      Data     { get; }
  }
}