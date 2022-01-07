using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Shaders;

namespace Surreal.Internal.Graphics.Resources;

/// <summary>A single shader, unliked to a program.</summary>
internal sealed record OpenTKShader(ShaderType Type, string Code);

/// <summary>A set of <see cref="OpenTKShader"/>s.</summary>
internal sealed record OpenTKShaderSet(string Path, ImmutableArray<OpenTKShader> Shaders) : ICompiledShaderProgram;
