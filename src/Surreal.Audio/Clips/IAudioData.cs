using System;
using Surreal.IO;

namespace Surreal.Audio.Clips {
  public interface IAudioData {
    TimeSpan Duration { get; }

    int SampleRate    { get; }
    int Channels      { get; }
    int BitsPerSample { get; }

    Size       Size { get; }
    Span<byte> Span { get; }
  }
}