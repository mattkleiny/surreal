using OpenTK;

namespace Surreal.Platform.Internal.Graphics.Resources
{
  internal sealed class ShaderProgramException : PlatformException
  {
    public ShaderProgramException(string message, string infoLog)
        : base(message)
    {
      InfoLog = infoLog;
    }

    public string InfoLog { get; }
  }
}