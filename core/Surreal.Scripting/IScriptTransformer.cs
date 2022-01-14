namespace Surreal.Scripting;

/// <summary>Transforms <see cref="ScriptDeclaration"/>s into different forms.</summary>
public interface IScriptTransformer
{
  bool CanTransform(ScriptDeclaration declaration);

  ValueTask<ScriptDeclaration> TransformAsync(ScriptDeclaration declaration, CancellationToken cancellationToken = default);
}
