using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Surreal.Collections;

namespace Surreal.IO {
  public static class FileSystems {
    public static readonly IFileSystemRegistry Registry = new FileSystemRegistry();

    public static IFileSystem GetForScheme(string scheme) => Registry.GetByScheme(scheme).FirstOrDefault();

    private sealed class FileSystemRegistry : IFileSystemRegistry {
      private readonly IMultiDictionary<string, IFileSystem> fileSystemByScheme = new MultiDictionary<string, IFileSystem>(StringComparer.OrdinalIgnoreCase);

      public void Add(IFileSystem system) {
        foreach (var scheme in system.Schemes) {
          fileSystemByScheme.Add(scheme, system);
        }
      }

      public void Clear() => fileSystemByScheme.Clear();

      public IEnumerable<IFileSystem> GetByScheme(string scheme) => fileSystemByScheme[scheme];

      public IEnumerator<IFileSystem> GetEnumerator() => fileSystemByScheme.Values.GetEnumerator();
      IEnumerator IEnumerable.        GetEnumerator() => GetEnumerator();
    }
  }
}