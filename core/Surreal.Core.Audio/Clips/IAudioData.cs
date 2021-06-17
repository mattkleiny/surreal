using System;
using Surreal.IO;

namespace Surreal.Audio.Clips {
  public interface IAudioData {
    TimeSpan        Duration { get; }
    AudioSampleRate Rate     { get; }
    Size            Size     { get; }
    Span<byte>      Span     { get; }
  }
}