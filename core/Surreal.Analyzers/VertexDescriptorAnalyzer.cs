using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Surreal;

/// <summary>
/// An analyzer that checks for correct usages of Vertex Descriptors
/// </summary>
[UsedImplicitly]
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class VertexDescriptorAnalyzer : DiagnosticAnalyzer
{
  private static readonly DiagnosticDescriptor VertexTypeNonSequential = new(
    id: "SUR0001",
    title: "The vertex type is not laid out sequentially",
    messageFormat: "The vertex type {0} is not laid out sequentially",
    category: "Surreal",
    defaultSeverity: DiagnosticSeverity.Error,
    isEnabledByDefault: true
  );

  private static readonly DiagnosticDescriptor VertexFieldMissingAttribute = new(
    id: "SUR0002",
    title: "The vertex field is missing a descriptor attribute",
    messageFormat: "The vertex field {0} is missing a descriptor attribute",
    category: "Surreal",
    defaultSeverity: DiagnosticSeverity.Error,
    isEnabledByDefault: true
  );

  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
    VertexTypeNonSequential,
    VertexFieldMissingAttribute
  );

  public override void Initialize(AnalysisContext context)
  {
    context.EnableConcurrentExecution();
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
  }
}
