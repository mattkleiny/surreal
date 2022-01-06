using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Shaders;

namespace Surreal.Internal.Graphics.Resources;

/// <summary>A single shader, unliked to a program.</summary>
internal sealed record OpenTkShader(string Code, ShaderType Type);

/// <summary>A set of <see cref="OpenTkShader"/>s.</summary>
internal sealed record OpenTkShaderSet(string FileName, string Description, OpenTkShader[] Shaders) : ICompiledShaderProgram;
