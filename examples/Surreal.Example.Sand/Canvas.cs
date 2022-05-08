namespace Surreal;

/// <summary>A simple canvas of 'sand' pixels that can be simulated.</summary>
public sealed class Canvas : PixelCanvas
{
  private readonly Grid<Cell> cells;

  private IntervalTimer updateTimer = new(16.Milliseconds());

  public Canvas(IGraphicsServer server, int width, int height)
    : base(server, width, height)
  {
    cells = new Grid<Cell>(width, height);
  }

  public void AddSand(Point2 position, int radius, Color32 color)
  {
    cells.Span.DrawCircle(position, radius, new Cell(IsOccupied: true, color));
  }

  public void DeleteSand(Point2 position, int radius)
  {
    cells.Span.DrawCircle(position, radius, new Cell(IsOccupied: false, Color32.Clear));
  }

  public void Update(TimeDelta deltaTime)
  {
    if (updateTimer.Tick(deltaTime))
    {
      Simulate();
      BlitToPixels();

      updateTimer.Reset();
    }
  }

  private void Simulate()
  {
    var cells = this.cells.Span;

    for (int y = cells.Height - 1; y >= 0; y--)
    for (int x = 0; x < cells.Width; x++)
    {
      ref var cell = ref cells[x, y];

      if (cell.IsOccupied)
      {
        if (SimulateSand(ref cell, x, y + 1)) continue;
        if (SimulateSand(ref cell, x - 1, y + 1)) continue;
        SimulateSand(ref cell, x + 1, y + 1);
      }
    }
  }

  private bool SimulateSand(ref Cell cell, int x, int y)
  {
    if (x < 0 || x > cells.Width - 1) return false;
    if (y < 0 || y > cells.Height - 1) return false;

    ref var target = ref cells[x, y];

    if (cell.IsOccupied && !target.IsOccupied)
    {
      target.IsOccupied = true;
      target.Color      = cell.Color;

      cell.IsOccupied = false;
      cell.Color      = Color32.Clear;

      return true;
    }

    return false;
  }

  private void BlitToPixels()
  {
    static Color32 Painter(int x, int y, Cell cell)
    {
      return cell.IsOccupied ? cell.Color : Color32.White;
    }

    cells.Span.PaintTo(Pixels, Painter);
  }

  public void Clear()
  {
    cells.Span.Fill(default);
  }

  private record struct Cell(bool IsOccupied, Color32 Color);
}
