using OpenTK.Graphics.OpenGL;

namespace Surreal.Internal.Graphics.Resources;

/// <summary>A single shader, unliked to a program.</summary>
internal sealed record OpenTkShader(string Code, ShaderType Type);
