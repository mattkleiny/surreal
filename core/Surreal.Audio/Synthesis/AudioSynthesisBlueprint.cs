using Surreal.Graphs;
using Surreal.Mathematics;
using Surreal.Objects;
using Surreal.Utilities;

namespace Surreal.Audio.Synthesis;

public sealed record AudioSynthesisPlan(Random Random);

public abstract record AudioSynthesisNode : GraphNode<AudioSynthesisNode>
{
  public virtual void Plan(AudioSynthesisPlan plan)
  {
    foreach (var child in Children)
    {
      child.Plan(plan);
    }
  }
}

[EditorDescription(
  Name = "Audio Synthesis Blueprint",
  Category = "Audio",
  Description = "A blueprint for audio synthesis"
)]
public sealed record AudioSynthesisBlueprint : Graph<AudioSynthesisNode>
{
  public AudioSynthesisPlan Create(Seed seed)
  {
    var plan = new AudioSynthesisPlan(seed.ToRandom());

    foreach (var child in Children)
    {
      child.Plan(plan);
    }

    return plan;
  }

  [Template(typeof(AudioSynthesisBlueprint))]
  public sealed record Template : ITemplate<AudioSynthesisBlueprint>
  {
    public AudioSynthesisBlueprint Create() => new();
  }
}

[EditorDescription(
  Name = "Sin Wave",
  Category = "Audio",
  Description = "Adds a simple sine wave to the waveform output"
)]
public sealed record SinWaveNode(float Frequency, float Amplitude) : AudioSynthesisNode
{
  public override void Plan(AudioSynthesisPlan plan)
  {
    throw new NotImplementedException();
  }

  [Template(typeof(SinWaveNode))]
  public sealed record Template : ITemplate<SinWaveNode>
  {
    public float Frequency { get; set; } = 16_000;
    public float Amplitude { get; set; } = 400;

    public SinWaveNode Create() => new(Frequency, Amplitude);
  }
}
