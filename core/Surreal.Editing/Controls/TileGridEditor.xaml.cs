using System.Windows;
using System.Windows.Media;
using Surreal.Mathematics;

namespace Surreal.Controls;

/// <summary>An editor for discrete tile grids.</summary>
public partial class TileGridEditor
{
  private float    gridSpacing = 48f;
  private Point2 gridSize    = new(15, 9);

  public TileGridEditor()
  {
    InitializeComponent();
  }

  public Point2 GridSize
  {
    get => gridSize;
    set
    {
      gridSize = value;
      InvalidateVisual();
    }
  }

  public float GridSpacing
  {
    get => gridSpacing;
    set
    {
      gridSpacing = value;
      InvalidateVisual();
    }
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    base.OnRender(drawingContext);

    // TODO: how to get full size here?

    var pen = new Pen(Brushes.Black, 2f);

    // horizontal lines
    for (var y = 0; y < gridSize.Y; y++)
    for (var x = 0; x < gridSize.X; x++)
    {
      drawingContext.DrawLine(pen, new Point(0, y * gridSpacing), new Point(gridSize.X * gridSpacing, y * gridSpacing));
    }

    // vertical lines
    for (var y = 0; y < gridSize.Y; y++)
    for (var x = 0; x < gridSize.X; x++)
    {
      drawingContext.DrawLine(pen, new Point(x * gridSpacing, 0), new Point(x * gridSpacing, gridSize.Y * gridSpacing));
    }
  }
}
