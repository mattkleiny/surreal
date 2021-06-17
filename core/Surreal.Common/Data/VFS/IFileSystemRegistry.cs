using Surreal.Collections;

namespace Surreal.Data.VFS {
  public interface IFileSystemRegistry {
    ReadOnlySlice<IFileSystem> GetByScheme(string scheme);

    void Add(IFileSystem system);
    void Clear();
  }
}