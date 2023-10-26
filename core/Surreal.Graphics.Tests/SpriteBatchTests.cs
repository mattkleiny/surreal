using Surreal.Graphics.Materials;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics;

public class SpriteBatchTests
{
  [Test]
  public void it_should_not_flush_when_texture_is_same()
  {
    var backend = IGraphicsBackend.Headless;

    using var batch = new SpriteBatch(backend);
    using var shader = new ShaderProgram(backend);
    using var material = new Material(backend, shader);
    using var texture1 = new Texture(backend, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);

    var flushCount = 0;

    batch.Flushed += () => Interlocked.Increment(ref flushCount);

    batch.Reset();

    batch.Draw(texture1, Vector2.Zero);
    batch.Draw(texture1, Vector2.Zero);
    batch.Draw(texture1, Vector2.Zero);
    batch.Draw(texture1, Vector2.Zero);
    batch.Draw(texture1, Vector2.Zero);
    batch.Draw(texture1, Vector2.Zero);

    batch.Flush();

    flushCount.Should().Be(1);
  }

  [Test]
  public void it_should_flush_when_texture_is_changing()
  {
    var backend = IGraphicsBackend.Headless;

    using var batch = new SpriteBatch(backend);
    using var shader = new ShaderProgram(backend);
    using var material = new Material(backend, shader);
    using var texture1 = new Texture(backend, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);
    using var texture2 = new Texture(backend, TextureFormat.Rgba8, TextureFilterMode.Point, TextureWrapMode.ClampToEdge);

    var flushCount = 0;

    batch.Flushed += () => Interlocked.Increment(ref flushCount);

    batch.Reset();

    batch.Draw(texture1, Vector2.Zero);
    batch.Draw(texture1, Vector2.Zero);
    batch.Draw(texture1, Vector2.Zero);
    batch.Draw(texture2, Vector2.Zero);
    batch.Draw(texture2, Vector2.Zero);
    batch.Draw(texture2, Vector2.Zero);

    batch.Flush();

    flushCount.Should().Be(2);
  }
}
