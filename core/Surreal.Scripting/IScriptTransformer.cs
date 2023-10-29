namespace Surreal.Scripting;

/// <summary>
/// Transforms <see cref="ScriptDeclaration"/>s into different forms.
/// </summary>
public interface IScriptTransformer
{
  /// <summary>
  /// Determines if the <see cref="IScriptTransformer"/> can transform the <see cref="ScriptDeclaration"/>.
  /// </summary>
  bool CanTransform(ScriptDeclaration declaration);

  /// <summary>
  /// Transforms the <see cref="ScriptDeclaration"/> into a different form.
  /// </summary>
  ValueTask<ScriptDeclaration> TransformAsync(ScriptDeclaration declaration, CancellationToken cancellationToken = default);
}
