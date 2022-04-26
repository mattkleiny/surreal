using System.Runtime.CompilerServices;
using Surreal.Memory;
using Surreal.Pixels;

namespace Sand;

/// <summary>A simple canvas of 'sand' pixels that can be simulated.</summary>
public sealed class SandCanvas : IDisposable
{
  private readonly Grid<Cell> cells;
  private readonly PixelCanvas pixels;
  private readonly Random random = new();

  private IntervalTimer updateTimer = new(16.Milliseconds());

  public SandCanvas(IGraphicsServer server, int width, int height)
  {
    cells  = new Grid<Cell>(width, height);
    pixels = new PixelCanvas(server, width, height);
  }

  public int Width  => pixels.Width;
  public int Height => pixels.Height;

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

      updateTimer.Reset();
    }
  }

  private void Simulate()
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool CheckAndMove(SpanGrid<Cell> cells, int x, int y, ref Cell cell)
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

    var cells = this.cells.Span;

    for (int y = cells.Height - 1 - 1; y >= 0; y--)
    for (int x = 0; x < cells.Width - 1; x++)
    {
      ref var cell = ref cells[x, y];

      if (cell.IsOccupied)
      {
        if (CheckAndMove(cells, x, y + 1, ref cell)) continue;

        if (random.NextBool())
        {
          if (CheckAndMove(cells, x - 1, y + 1, ref cell)) continue;
          CheckAndMove(cells, x + 1, y + 1, ref cell);
        }
        else
        {
          if (CheckAndMove(cells, x + 1, y + 1, ref cell)) continue;
          CheckAndMove(cells, x - 1, y + 1, ref cell);
        }
      }
    }
  }

  public void Draw(ShaderProgram shader)
  {
    static Color Painter(int x, int y, Cell cell)
    {
      return cell.IsOccupied ? cell.Color : Color.Black;
    }

    cells.Span.Blit(pixels.Span, Painter);
    pixels.Draw(shader);
  }

  public void Dispose()
  {
    pixels.Dispose();
  }

  private record struct Cell(bool IsOccupied, Color32 Color);
}
