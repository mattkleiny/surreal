using Surreal.Graphics.Shaders;

namespace Surreal.Internal.Graphics.Resources;

internal record HeadlessCompiledShader(string FileName, string Description) : ICompiledShaderProgram;
