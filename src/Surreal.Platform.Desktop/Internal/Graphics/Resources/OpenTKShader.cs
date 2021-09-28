using OpenTK.Graphics.OpenGL;

namespace Surreal.Platform.Internal.Graphics.Resources
{
  internal readonly record struct OpenTKShader(string Code, ShaderType Type);
}
