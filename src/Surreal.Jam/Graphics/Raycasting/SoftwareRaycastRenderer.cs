using System;
using System.Numerics;
using Surreal.Collections;
using Surreal.Framework.Tiles;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Raycasting {
  public class SoftwareRaycastRenderer<TTile> : IDisposable
      where TTile : IRaycastAwareTile {
    private readonly RaycastCamera      camera;
    private readonly Color              clearColor;
    private readonly Atlas<ImageRegion> textures;

    public SoftwareRaycastRenderer(
        RaycastCamera camera,
        Vector2I resolution,
        Atlas<ImageRegion> textures,
        Color clearColor = default) {
      this.camera     = camera;
      this.textures   = textures;
      this.clearColor = clearColor;

      FrameBuffer = new SoftwareFrameBuffer(resolution.X, resolution.Y);
    }

    public SoftwareFrameBuffer FrameBuffer { get; }

    public void Render(IGraphicsDevice device, SpriteBatch batch, TileMap<TTile> map) {
      var scale = new Vector2(
          (float) FrameBuffer.Width / map.Width,
          (float) FrameBuffer.Height / map.Height
      );

      BeforeRender(scale);
      RenderMap(map, scale);
      AfterRender(scale);

      FrameBuffer.Draw(batch);
    }

    protected virtual void BeforeRender(Vector2 scale) {
      FrameBuffer.Clear(clearColor);
    }

    protected virtual void RenderMap(TileMap<TTile> map, Vector2 scale) {
      // prepare camera and viewport
      var position  = camera.Position;
      var direction = camera.Direction;

      var viewWidth  = (float) FrameBuffer.Width / FrameBuffer.Height;
      var viewPlane  = direction.Orthogonal() * viewWidth;
      var viewCenter = position + direction * camera.FocalLength;
      var viewStart  = viewCenter - viewPlane / 2f;

      var columns = FrameBuffer.Width;
      var rows    = FrameBuffer.Height;

      var step      = viewPlane / columns;
      var columnPos = viewStart;

      for (var x = 0; x < columns; x++) {
        // compute rays and sample tiles
        var rayDirection      = columnPos - position;
        var viewPlaneDistance = rayDirection.Length();

        var ray  = new Ray(position, rayDirection / viewPlaneDistance);
        var end  = map.CastRay(ray);
        var tile = map.GetNearestTile(new Ray(end, ray.Direction));

        // compute wall properties
        var wallDistance  = (end - ray.Origin).Length();
        var distanceRatio = viewPlaneDistance / camera.FocalLength;
        var perpendicular = wallDistance / distanceRatio;
        var height        = camera.WallHeight * camera.FocalLength / perpendicular * rows;
        var wallStart     = new Vector2(x, (rows - height) / 2f + camera.FudgeBias);

        // darken walls and fix texture stretching
        Color dampen;
        float wallX;

        if (Math.Abs(MathF.Floor(end.X) - end.X) < float.Epsilon) {
          dampen = Color.Clear;
          wallX  = end.Y - MathF.Floor(end.Y);
        } else {
          dampen = new Color(50, 50, 50, 0);
          wallX  = end.X - MathF.Floor(end.X);
        }

        // draw walls
        if (tile.Texture != null) {
          var texture  = textures[tile.Texture];
          var textureX = (int) (wallX * texture.Width);

          FrameBuffer.Colors.DrawTexturedColumn(textureX, texture, wallStart, height, dampen);
        } else {
          FrameBuffer.Colors.DrawColoredColumn(x, wallStart, height, tile.Color - dampen);
        }

        // draw floors
        var floorTexture = textures["textures_1"];
        var floorStart   = (int) (wallStart.Y + height) + 1;

        for (var y = Math.Min(floorStart, rows); y < rows; y++) {
          var normalizedY       = y / rows * 2 - 1;
          var wallPerpendicular = camera.WallHeight * camera.FocalLength / normalizedY;
          var distance          = wallPerpendicular * distanceRatio;
          var mapPosition       = ray.Origin + ray.Direction * distance;

          var tileX = (int) MathF.Floor(mapPosition.X);
          var tileY = (int) MathF.Floor(mapPosition.Y);

          var textureX = (int) (mapPosition.X - tileX) * floorTexture.Width;
          var textureY = (int) (mapPosition.Y - tileY) * floorTexture.Height;

          FrameBuffer.Colors[x, y] = floorTexture[textureX, textureY];
        }

        // TODO: draw ceilings
        columnPos += step;
      }
    }

    protected virtual void AfterRender(Vector2 scale) {
    }

    public void Dispose() {
      FrameBuffer.Dispose();
    }
  }
}