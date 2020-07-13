using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Sprites {
  public sealed class SpriteSheetCutter : IDisposable {
    private readonly Queue<ImageRegion> availableCells;

    public SpriteSheetCutter(int width, int height, int widthPerCell, int heightPerCell, Color initialColor = default) {
      Debug.Assert(width         > 0, "width > 0");
      Debug.Assert(height        > 0, "height > 0");
      Debug.Assert(widthPerCell  > 0, "widthPerCell > 0");
      Debug.Assert(heightPerCell > 0, "heightPerCell > 0");

      Image = new Image(width, height);
      Image.Fill(initialColor);

      var horizontalCells = width  / widthPerCell;
      var verticalCells   = height / heightPerCell;

      availableCells = new Queue<ImageRegion>(horizontalCells * verticalCells);

      for (var y = 0; y < verticalCells; y++)
      for (var x = 0; x < horizontalCells; x++) {
        var cell = Image.ToRegion().Slice(
            x * horizontalCells,
            y * verticalCells,
            widthPerCell,
            heightPerCell
        );

        availableCells.Enqueue(cell);
      }
    }

    public Image Image { get; }

    public IPlannedSprite AddSprite() {
      lock (availableCells) {
        var cell = availableCells.Dequeue();

        return new PlannedSprite(cell);
      }
    }

    public IPlannedSpriteAnimation AddSpriteAnimation(int cellCount) {
      Debug.Assert(cellCount > 0, "cellCount > 0");

      lock (availableCells) {
        var cells = Enumerable
            .Range(0, cellCount)
            .Select(_ => availableCells.Dequeue())
            .ToArray();

        return new PlannedSpriteAnimation(cells);
      }
    }

    public void Dispose() {
      Image.Dispose();
    }

    private sealed class PlannedSprite : IPlannedSprite {
      public PlannedSprite(ImageRegion region) {
        Region = region;
      }

      public ImageRegion Region { get; }

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
      private readonly ImageRegion[] regions;

      public PlannedSpriteAnimation(ImageRegion[] regions) {
        this.regions = regions;
      }

      public int  CellCount       => regions.Length;
      public bool IsLooping       { get; set; }
      public bool IsCycling       { get; set; }
      public int  FramesPerSecond { get; set; }

      public IPlannedSprite this[int index] {
        get {
          Debug.Assert(index >= 0, "index >= 0");
          Debug.Assert(index < regions.Length, "index < regions.Length");

          return new PlannedSprite(regions[index]);
        }
      }
    }
  }
}