﻿using Surreal.Resources;

namespace Surreal.Audio.Clips;

public class AudioBufferLoaderTests
{
  [Test]
  [TestCase("Assets/audio/test.wav")]
  public async Task it_should_load_an_audio_buffer(string path)
  {
    using var manager = new ResourceManager();

    manager.AddLoader(new AudioBufferLoader());

    var buffer = await manager.LoadResourceAsync<AudioBuffer>(path);

    buffer.Rate.Frequency.Should().BeGreaterThan(0);
    buffer.Memory.Length.Should().BeGreaterThan(0);
  }
}
