using System;
using System.Collections.Generic;
using System.Linq;
using Surreal.Graphics.Textures;
using Surreal.Mathematics.Grids;

namespace Surreal.Graphics.Sprites {
  public sealed class SpriteSheetCutter : IDisposable {
    private readonly Queue<PixmapRegion> availableCells;

    public SpriteSheetCutter(int width, int height, int widthPerCell, int heightPerCell, Color initialColor = default) {
      Check.That(width         > 0, "width > 0");
      Check.That(height        > 0, "height > 0");
      Check.That(widthPerCell  > 0, "widthPerCell > 0");
      Check.That(heightPerCell > 0, "heightPerCell > 0");

      Pixmap = new Pixmap(width, height);
      Pixmap.Fill(initialColor);

      var horizontalCells = width  / widthPerCell;
      var verticalCells   = height / heightPerCell;

      availableCells = new Queue<PixmapRegion>(horizontalCells * verticalCells);

      for (var y = 0; y < verticalCells; y++)
      for (var x = 0; x < horizontalCells; x++) {
        var cell = Pixmap.ToRegion().Slice(
            x * horizontalCells,
            y * verticalCells,
            widthPerCell,
            heightPerCell
        );

        availableCells.Enqueue(cell);
      }
    }

    public Pixmap Pixmap { get; }

    public IPlannedSprite AddSprite() {
      lock (availableCells) {
        var cell = availableCells.Dequeue();

        return new PlannedSprite(cell);
      }
    }

    public IPlannedSpriteAnimation AddSpriteAnimation(int cellCount) {
      Check.That(cellCount > 0, "cellCount > 0");

      lock (availableCells) {
        var cells = Enumerable
            .Range(0, cellCount)
            .Select(_ => availableCells.Dequeue())
            .ToArray();

        return new PlannedSpriteAnimation(cells);
      }
    }

    public void Dispose() {
      Pixmap.Dispose();
    }

    private sealed class PlannedSprite : IPlannedSprite {
      public PlannedSprite(PixmapRegion region) {
        Region = region;
      }

      public PixmapRegion Region { get; }

      public int Width   => Region.Width;
      public int Height  => Region.Height;
      public int OffsetX => Region.OffsetX;
      public int OffsetY => Region.OffsetY;

      public Color this[int x, int y] {
        get => Region[x, y];
        set => Region[x, y] = value;
      }
    }

    private sealed class PlannedSpriteAnimation : IPlannedSpriteAnimation {
      private readonly PixmapRegion[] regions;

      public PlannedSpriteAnimation(PixmapRegion[] regions) {
        this.regions = regions;
      }

      public int  CellCount       => regions.Length;
      public bool IsLooping       { get; set; }
      public bool IsCycling       { get; set; }
      public int  FramesPerSecond { get; set; }

      public IPlannedSprite this[int index] {
        get {
          Check.That(index >= 0, "index >= 0");
          Check.That(index < regions.Length, "index < regions.Length");

          return new PlannedSprite(regions[index]);
        }
      }
    }
  }
}