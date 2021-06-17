using System.Collections.Generic;

namespace Surreal.IO.VFS {
  public interface IFileSystemRegistry : IEnumerable<IFileSystem> {
    IEnumerable<IFileSystem> GetByScheme(string scheme);

    void Add(IFileSystem system);
    void Clear();
  }
}