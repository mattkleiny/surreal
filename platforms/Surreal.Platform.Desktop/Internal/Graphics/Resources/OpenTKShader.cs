using OpenTK.Graphics.OpenGL;

namespace Surreal.Internal.Graphics.Resources;

internal readonly record struct OpenTKShader(string Code, ShaderType Type);
