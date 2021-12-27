using System.Diagnostics;
using OpenTK.Core.Platform;
using OpenTK.Graphics.OpenGL;

namespace Surreal.Internal.Graphics;

internal sealed class OpenTkGraphicsException : PlatformException
{
	[Conditional("DEBUG")]
	public static void CheckAndThrow()
	{
		var errorCode = GL.GetError();
		if (errorCode != ErrorCode.NoError)
		{
			throw new OpenTkGraphicsException(errorCode switch
			{
				ErrorCode.InvalidEnum => "An invalid OpenGL enum was passed.",
				ErrorCode.InvalidValue => "An invalid OpenGL value was passed.",
				ErrorCode.InvalidOperation => "An invalid OpenGL operation was attempted.",
				ErrorCode.StackOverflow => "The OpenGL stack has overflowed.",
				ErrorCode.StackUnderflow => "The OpenGL stack has underflowed.",
				ErrorCode.OutOfMemory => "OpenGL is out of memory.",
				ErrorCode.InvalidFramebufferOperation => "An invalid OpenGL frame buffer operation was attempted.",
				ErrorCode.ContextLost => "The OpenGL context was lost.",
				ErrorCode.TableTooLarge => "The OpenGL table is too large.",
				ErrorCode.TextureTooLargeExt => "The OpenGL texture is too large.",
				_ => "An unexpected OpenGL error occurred."
			});
		}
	}

	public OpenTkGraphicsException(string message)
		: base(message)
	{
	}
}
