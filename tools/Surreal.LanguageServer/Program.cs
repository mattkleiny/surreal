using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using Surreal;
using Surreal.Handlers;

var server = await LanguageServer.From(options =>
{
  options.WithInput(Console.OpenStandardInput());
  options.WithOutput(Console.OpenStandardOutput());

  options.WithHandler<TextDocumentHandler>();

  options.OnStarted(async (languageServer, cancellationToken) =>
  {
    using var work = await languageServer.WorkDoneManager.Create(new WorkDoneProgressBegin
    {
      Title = "Doing some work"
    });

    work.OnNext(new WorkDoneProgressReport { Message = "doing things..." });

    await Task.Delay(10000, cancellationToken);

    work.OnNext(new WorkDoneProgressReport { Message = "doing things... 1234" });

    await Task.Delay(10000, cancellationToken);

    work.OnNext(new WorkDoneProgressReport { Message = "doing things... 56789" });
  });
});

await server.WaitForExit;
