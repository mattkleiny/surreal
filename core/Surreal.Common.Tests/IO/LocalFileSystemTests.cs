namespace Surreal.IO;

public class LocalFileSystemTests
{
  [Test]
  [TestCase("test1.txt")]
  [TestCase("./test1.txt")]
  [TestCase("./test/test1.txt")]
  [TestCase(@"\test\test1.txt")]
  [TestCase(@"C:\test\test1.txt")]
  public void it_should_determine_valid_file_names(string path)
  {
    var system = new LocalFileSystem();

    system.IsFile(path).Should().BeTrue();
  }

  [Test]
  [TestCase("test1txt")]
  [TestCase("./test1txt")]
  [TestCase("./test/test1txt")]
  [TestCase(@"\test\test1txt")]
  [TestCase(@"C:\test\test1txt")]
  public void it_should_determine_invalid_file_names(string path)
  {
    var system = new LocalFileSystem();

    system.IsFile(path).Should().BeFalse();
  }

  [Test]
  [TestCase("test1.txt")]
  [TestCase("./test1.txt")]
  [TestCase("./test/test1.txt")]
  [TestCase(@"\test\test1.txt")]
  [TestCase(@"C:\test\test1.txt")]
  public void it_should_determine_valid_directory_names(string path)
  {
    var system = new LocalFileSystem();

    system.IsDirectory(path).Should().BeFalse();
  }

  [Test]
  [TestCase("test1txt")]
  [TestCase("./test1txt")]
  [TestCase("./test/test1txt")]
  [TestCase(@"\test\test1txt")]
  [TestCase(@"C:\test\test1txt")]
  public void it_should_determine_invalid_directory_names(string path)
  {
    var system = new LocalFileSystem();

    system.IsDirectory(path).Should().BeTrue();
  }
}
