using Surreal.Graphs;
using Surreal.Mathematics;
using Surreal.Memory;
using Surreal.Objects;
using Surreal.Utilities;

namespace Surreal.Audio.Synthesis;

public sealed record AudioSynthesisKernel
{
  private readonly List<Action<IBuffer<float>>> actions = new();

  public void AddStep(Action<IBuffer<float>> action)
  {
    actions.Add(action);
  }

  public void Execute(IBuffer<float> buffer)
  {
    foreach (var action in actions)
    {
      action(buffer);
    }
  }
}

public abstract record AudioSynthesisNode : GraphNode<AudioSynthesisNode>
{
  public virtual void Plan(AudioSynthesisKernel kernel)
  {
    foreach (var child in Children)
    {
      child.Plan(kernel);
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
  public AudioSynthesisKernel Create(Seed seed)
  {
    var plan = new AudioSynthesisKernel();

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
  public override void Plan(AudioSynthesisKernel kernel)
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
