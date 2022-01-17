﻿using Surreal.Graphics.Shaders;
using Surreal.IO;

namespace Surreal.Internal.Graphics;

public class OpenTKGraphicsServerTests
{
  [Test, Ignore("Not yet implemented")]
  [TestCase("Assets/shaders/test01.shade")]
  public async Task it_should_compile_a_shader(VirtualPath path)
  {
    var parser   = new StandardShaderParser();
    var pipeline = new OpenTKGraphicsServer();

    var shader = await parser.ParseShaderAsync(path);

    var id = pipeline.Shaders.CreateShader();

    pipeline.Shaders.CompileShader(id, shader);
  }
}
