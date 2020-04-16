using System.IO;
using System.Reflection;
using Surreal.IO;
using Xunit;

namespace Surreal.Core.IO
{
  public class ResourceFileSystemTests
  {
    [Fact]
    public async void it_should_load_files_from_embedded_resources()
    {
      var fileSystem = new ResourceFileSystem(Assembly.GetExecutingAssembly());

      await using (var stream = await fileSystem.OpenInputStreamAsync("Surreal.Core.Resources.Test.txt"))
      using (var reader = new StreamReader(stream))
      {
        var result = await reader.ReadToEndAsync();

        Assert.Equal("Hello, World!", result);
      }
    }
  }
}