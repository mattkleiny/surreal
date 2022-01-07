﻿using OpenTK.Graphics.OpenGL;

namespace Surreal.Internal.Graphics;

internal sealed class OpenTKGraphicsException : PlatformException
{
  [Conditional("DEBUG")]
  public static void CheckAndThrow()
  {
    var errorCode = GL.GetError();
    if (errorCode != ErrorCode.NoError)
    {
      throw new OpenTKGraphicsException(errorCode switch
      {
        ErrorCode.InvalidEnum                 => "An invalid OpenGL enum was passed.",
        ErrorCode.InvalidValue                => "An invalid OpenGL value was passed.",
        ErrorCode.InvalidOperation            => "An invalid OpenGL operation was attempted.",
        ErrorCode.StackOverflow               => "The OpenGL stack has overflowed.",
        ErrorCode.StackUnderflow              => "The OpenGL stack has underflowed.",
        ErrorCode.OutOfMemory                 => "OpenGL is out of memory.",
        ErrorCode.InvalidFramebufferOperation => "An invalid OpenGL frame buffer operation was attempted.",
        ErrorCode.TableTooLarge               => "The OpenGL table is too large.",
        ErrorCode.TextureTooLargeExt          => "The OpenGL texture is too large.",
        _                                     => "An unexpected OpenGL error occurred.",
      });
    }
  }

  public OpenTKGraphicsException(string message)
    : base(message)
  {
  }
}