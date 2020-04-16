using System;

namespace Surreal.Assets
{
  public class UnsupportedAssetException : Exception
  {
    public UnsupportedAssetException(string message)
      : base(message)
    {
    }
  }
}
