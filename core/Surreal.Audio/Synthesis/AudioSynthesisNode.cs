using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Surreal.Graphs;
using Surreal.Mathematics;

namespace Surreal.Audio.Synthesis;

/// <summary>A single node in a graph used for audio synthesis.</summary>
public abstract record AudioSynthesisNode : GraphNode<AudioSynthesisNode>
{
  /// <summary>An action to be executed in the plan.</summary>
  private delegate int AudioSynthesisStep(Memory<byte> buffer);

  /// <summary>An output <see cref="AudioSynthesisNode"/>.</summary>
  public sealed record OutputNode : AudioSynthesisNode
  {
    public IAudioSynthesisPlan CreatePlan(Seed seed = default)
    {
      var plan = new AudioSynthesisPlan();

      foreach (var node in Children.OfType<WaveNode>())
      {
        plan.Steps.Push(buffer =>
        {
          var span = MemoryMarshal.Cast<byte, float>(buffer.Span);

          span[0] = node.Evaluate(0f);

          return Unsafe.SizeOf<float>();
        });
      }

      return plan;
    }
  }

  /// <summary>A <see cref="AudioSynthesisNode"/> that generates an audio wave</summary>
  public abstract record WaveNode : AudioSynthesisNode
  {
    /// <summary>Calculates the wave amount at the given <see cref="t"/>.</summary>
    public abstract float Evaluate(float t);

    public sealed record SinWave : WaveNode
    {
      public override float Evaluate(float t) => MathF.Sin(t);
    }

    public sealed record CosWave : WaveNode
    {
      public override float Evaluate(float t) => MathF.Cos(t);
    }

    public sealed record TanWave : WaveNode
    {
      public override float Evaluate(float t) => MathF.Tan(t);
    }
  }

  /// <summary>The default <see cref="IAudioSynthesisPlan"/> implementation.</summary>
  private sealed class AudioSynthesisPlan : IAudioSynthesisPlan
  {
    public Stack<AudioSynthesisStep> Steps { get; } = new();

    public async Task EmitAsync(IAudioSynthesisPipe pipe, CancellationToken cancellationToken = default)
    {
      await using var writer = pipe.CreateWriter();

      while (!cancellationToken.IsCancellationRequested)
      {
        var buffer       = writer.GetBuffer(4096);
        var bytesWritten = 0;

        foreach (var step in Steps)
        {
          bytesWritten += step(buffer);
        }

        writer.Advance(bytesWritten);

        await writer.FlushAsync();
      }
    }
  }
}
