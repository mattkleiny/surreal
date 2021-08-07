using System;
using System.IO;
using System.Threading.Tasks;
using Surreal.Memory;

namespace Surreal.IO
{
  /// <summary>A protocol for the <see cref="PackedFileSystem"/>.</summary>
  public interface IFilePackingProtocol
  {
  }

  /// <summary>A <see cref="FileSystem"/> that uses a <see cref="IFilePackingProtocol"/>.</summary>
  public sealed class PackedFileSystem : FileSystem
  {
    private readonly IFilePackingProtocol protocol;

    public PackedFileSystem(IFilePackingProtocol protocol)
        : base("pak")
    {
      this.protocol = protocol;
    }

    public override Path Resolve(string root, params string[] paths)
    {
      throw new NotImplementedException();
    }

    public override ValueTask<Path[]> EnumerateAsync(string path, string wildcard)
    {
      throw new NotImplementedException();
    }

    public override ValueTask<Size> GetSizeAsync(string path)
    {
      throw new NotImplementedException();
    }

    public override ValueTask<bool> IsDirectoryAsync(string path)
    {
      throw new NotImplementedException();
    }

    public override ValueTask<bool> ExistsAsync(string path)
    {
      throw new NotImplementedException();
    }

    public override ValueTask<bool> IsFileAsync(string path)
    {
      throw new NotImplementedException();
    }

    public override ValueTask<Stream> OpenInputStreamAsync(string path)
    {
      throw new NotImplementedException();
    }

    public override ValueTask<Stream> OpenOutputStreamAsync(string path)
    {
      throw new NotImplementedException();
    }
  }
}
