using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Shaders;

namespace Surreal.Internal.Graphics.Resources;

/// <summary>A single shader, unliked to a program.</summary>
internal sealed record OpenTKShader(string Code, ShaderType Type);

/// <summary>A set of <see cref="OpenTKShader"/>s.</summary>
internal sealed record OpenTKShaderSet(string FileName, string Description, OpenTKShader[] Shaders) : ICompiledShaderProgram;
