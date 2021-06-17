using System;
using Surreal.Collections;

namespace Surreal.Data.VFS {
  public interface IFileSystemRegistry {
    ReadOnlySlice<IFileSystem> GetByScheme(string scheme);

    void Add(IFileSystem system);
    void Clear();
  }

  public static class FileSystems {
    public static readonly IFileSystemRegistry Registry = new FileSystemRegistry();

    public static IFileSystem? GetForScheme(string scheme) {
      var systems = Registry.GetByScheme(scheme);

      if (systems.Length > 0) {
        return systems[0];
      }

      return default;
    }

    private sealed class FileSystemRegistry : IFileSystemRegistry {
      private readonly MultiDictionary<string, IFileSystem> fileSystemByScheme = new(StringComparer.OrdinalIgnoreCase);

      public ReadOnlySlice<IFileSystem> GetByScheme(string scheme) {
        return fileSystemByScheme[scheme];
      }

      public void Add(IFileSystem system) {
        foreach (var scheme in system.Schemes) {
          fileSystemByScheme.Add(scheme, system);
        }
      }

      public void Clear() {
        fileSystemByScheme.Clear();
      }
    }
  }
}