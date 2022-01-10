using Surreal.UI.Immediate;
using Surreal.UI.Widgets;

namespace Surreal.Immediate;

public class CanvasTests
{
  [Test, Ignore("Not yet implemented")]
  public void it_should_layout_a_simple_canvas()
  {
    var canvas = new StatisticsCanvas();

    Assert.DoesNotThrow(() => canvas.OnLayout());
  }

  [Test, Ignore("Not yet implemented")]
  public void it_should_paint_a_simple_canvas()
  {
    var canvas = new StatisticsCanvas();

    Assert.DoesNotThrow(() => canvas.OnPaint());
  }

  /// <summary>A simple immediate mode <see cref="Canvas"/>.</summary>
  private sealed class StatisticsCanvas : Canvas
  {
    private bool isEnabled;

    protected override void OnPaintCanvas(in PaintingContext context)
    {
      ref var state  = ref context.GetControlState<StatisticsState>();
      var     layout = context.BeginLayout();

      isEnabled = layout.Toggle("Show statistics", isEnabled);

      if (isEnabled)
      {
        layout.Label($"Frames per second: {state.FramesPerSecond:g2}");
        layout.Label($"Vertices on-screen: {state.VerticesOnScreen}");
        layout.Label($"Indices on-screen: {state.IndicesOnScreen}");
      }
    }

    /// <summary>State for our <see cref="StatisticsCanvas"/> paint operations.</summary>
    private struct StatisticsState
    {
      public float FramesPerSecond  { get; set; } = 144.0f;
      public uint  VerticesOnScreen { get; set; } = 10_000;
      public uint  IndicesOnScreen  { get; set; } = 20_000;
    }
  }
}
