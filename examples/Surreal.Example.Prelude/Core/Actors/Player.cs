using System;
using Surreal.Framework.Tiles;
using Surreal.Graphics.Raycasting;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;
using Surreal.Timing;

namespace Prelude.Core.Actors
{
  public class Player : PreludeActor
  {
    private readonly TileMap<Tile> map;
    private readonly RaycastCamera camera;

    public Player(TileMap<Tile> map, RaycastCamera camera)
    {
      this.map    = map;
      this.camera = camera;
    }

    public override void Input(DeltaTime deltaTime)
    {
      base.Input(deltaTime);

      var input = new Vector2I(0, 0);

      if (PreludeGame.Current.Keyboard.IsKeyDown(Key.W)) input.Y += 1;
      if (PreludeGame.Current.Keyboard.IsKeyDown(Key.S)) input.Y -= 1;
      if (PreludeGame.Current.Keyboard.IsKeyDown(Key.A)) input.X -= 1;
      if (PreludeGame.Current.Keyboard.IsKeyDown(Key.D)) input.X += 1;

      var rotation = input.X * TurningSpeed * deltaTime;
      var matrix = Matrix2x2.CreateFromAngles(
        MathF.Sin(rotation),
        MathF.Cos(rotation)
      );

      Direction = matrix * Direction;
      Velocity  = Direction * Speed * input.Y;
    }

    public override void Update(DeltaTime deltaTime)
    {
      base.Update(deltaTime);

      var oldPosition = Position;

      Position += Velocity * Speed * deltaTime;

      if (IsIntersecting(map))
      {
        Position = oldPosition;
      }

      camera.Position  = Position;
      camera.Direction = Direction;
    }

    private bool IsIntersecting(IGrid<Tile> map)
    {
      for (var y = (int) Bounds.Bottom; y <= (int) Bounds.Top; y++)
      for (var x = (int) Bounds.Left; x <= (int) Bounds.Right; x++)
      {
        if (map[x, y].IsSolid)
        {
          return true;
        }
      }

      return false;
    }
  }
}
