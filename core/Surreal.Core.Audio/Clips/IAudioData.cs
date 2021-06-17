using System;
using Surreal.Data;

namespace Surreal.Audio.Clips {
  public interface IAudioData {
    TimeSpan        Duration { get; }
    AudioSampleRate Rate     { get; }
    Size            Size     { get; }
    Span<byte>      Span     { get; }
  }
}