using Surreal.Graphics.Materials;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Sprites;

public class SpriteBatchTests
{
  [Test]
  public void it_should_not_flush_when_texture_is_same()
  {
    var device = IGraphicsDevice.Null;

    using var batch = new SpriteBatch(device);
    using var shader = new ShaderProgram(device);
    using var material = new Material(device, shader);
    using var texture1 = new Texture(device, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);

    var flushCount = 0;

    batch.Flushed += () => Interlocked.Increment(ref flushCount);

    batch.Reset();

    batch.DrawQuad(texture1, Vector2.Zero);
    batch.DrawQuad(texture1, Vector2.Zero);
    batch.DrawQuad(texture1, Vector2.Zero);
    batch.DrawQuad(texture1, Vector2.Zero);
    batch.DrawQuad(texture1, Vector2.Zero);
    batch.DrawQuad(texture1, Vector2.Zero);

    batch.Flush();

    flushCount.Should().Be(1);
  }

  [Test]
  public void it_should_flush_when_texture_is_changing()
  {
    var device = IGraphicsDevice.Null;

    using var batch = new SpriteBatch(device);
    using var shader = new ShaderProgram(device);
    using var material = new Material(device, shader);
    using var texture1 = new Texture(device, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);
    using var texture2 = new Texture(device, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);

    var flushCount = 0;

    batch.Flushed += () => Interlocked.Increment(ref flushCount);

    batch.Reset();

    batch.DrawQuad(texture1, Vector2.Zero);
    batch.DrawQuad(texture1, Vector2.Zero);
    batch.DrawQuad(texture1, Vector2.Zero);
    batch.DrawQuad(texture2, Vector2.Zero);
    batch.DrawQuad(texture2, Vector2.Zero);
    batch.DrawQuad(texture2, Vector2.Zero);

    batch.Flush();

    flushCount.Should().Be(2);
  }
}
