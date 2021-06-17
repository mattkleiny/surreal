using System;
using Surreal.Mathematics.Linear;

namespace Surreal.Modules.Tilemaps {
  public class TileMap<TTile> {
    public TTile this[TileLayer layer, int x, int y] {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public TTile this[TileLayer layer, Point2 point] {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }
  }
}