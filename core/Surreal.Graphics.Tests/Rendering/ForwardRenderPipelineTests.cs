using Surreal.Graphics.Materials;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Tests.Rendering;

public class ForwardRenderPipelineTests
{
  [Test]
  public void it_should_blit_color_buffer_after_render()
  {
    var backend = Substitute.For<IGraphicsBackend>();
    var scene = Substitute.For<IRenderScene>();
    var viewport = Substitute.For<IRenderViewport>();

    backend.IsActiveFrameBuffer(Arg.Any<FrameBufferHandle>()).Returns(true);
    scene.CullVisibleViewports().Returns(new[] { viewport });
    viewport.CullVisibleObjects<IRenderObject>().Returns(new[]
    {
      Substitute.For<IRenderObject>(),
      Substitute.For<IRenderObject>(),
      Substitute.For<IRenderObject>(),
    });

    using var pipeline = new ForwardRenderPipeline(backend);

    pipeline.Render(scene, TODO);

    backend.Received().BlitToBackBuffer(
      handle: Arg.Any<FrameBufferHandle>(),
      material: Arg.Any<Material>(),
      samplerProperty: Arg.Any<MaterialProperty<TextureSampler>>(),
      filterMode: Arg.Any<Optional<TextureFilterMode>>(),
      wrapMode: Arg.Any<Optional<TextureWrapMode>>()
    );
  }
}
