using Surreal.Timing;
using static Surreal.Audio.Synthesis.AudioSynthesisNode;
using static Surreal.Audio.Synthesis.AudioSynthesisNode.WaveNode;

namespace Surreal.Audio.Synthesis;

public class AudioSynthesizerTests
{
  [Test]
  public async Task it_should_generate_a_simple_wave()
  {
    var graph = new OutputNode { new CosWave() };
    var plan  = graph.CreatePlan();

    using var synthesizer = new AudioSynthesizer(plan);

    synthesizer.Start();

    await Task.Delay(10.Milliseconds());
  }
}
