using Surreal.Assets;

namespace Surreal.Scripting;

public class ScriptLoaderTests
{
  [Test, AutoFixture]
  public async Task it_should_load_script_with_delegated_parser(IScriptParser parser)
  {
    var manager = new AssetManager();

    manager.AddLoader(new ScriptLoader(parser, ".bas"));

    await manager.LoadAssetAsync<ScriptDeclaration>("Assets/scripts/test.bas");

    await parser.Received(1).ParseScriptAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<Encoding>(), Arg.Any<CancellationToken>());
  }
}
