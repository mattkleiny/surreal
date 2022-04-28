using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Surreal.Handlers;

public sealed class TextDocumentHandler : TextDocumentSyncHandlerBase
{
  public override Task<MediatR.Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
  {
    return MediatR.Unit.Task;
  }

  public override Task<MediatR.Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
  {
    return MediatR.Unit.Task;
  }

  public override Task<MediatR.Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
  {
    return MediatR.Unit.Task;
  }

  public override Task<MediatR.Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
  {
    return MediatR.Unit.Task;
  }

  protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities)
  {
    return new TextDocumentSyncRegistrationOptions
    {
    };
  }

  public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
  {
    return new TextDocumentAttributes(uri, "shadey");
  }
}
