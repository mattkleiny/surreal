using Surreal.Assets;

namespace Surreal.Scripting;

public class ScriptLoaderTests
{
  [Test, AutoFixture]
  public async Task it_should_load_script_with_delegated_parser(IScriptParser parser)
  {
    using var manager = new AssetManager();

    manager.AddLoader(new ScriptDeclarationLoader(parser, ".script"));

    await manager.LoadAssetAsync<ScriptDeclaration>("Assets/scripts/test.script");

    await parser.Received(1).ParseScriptAsync(Arg.Any<string>(), Arg.Any<TextReader>(), Arg.Any<CancellationToken>());
  }
}
