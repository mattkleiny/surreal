using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Surreal;

/// <summary>
/// An analyzer that checks for the use of <see cref="DateTime.Now" /> and <see cref="DateTime.UtcNow" />.
/// </summary>
[UsedImplicitly]
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ClockAnalyzer : DiagnosticAnalyzer
{
  private static readonly DiagnosticDescriptor ClockUsage = new(
    id: "SUR0001",
    title: "Do not use DateTime.Now or DateTime.UtcNow",
    messageFormat: "Do not use DateTime.Now or DateTime.UtcNow",
    category: "Surreal",
    defaultSeverity: DiagnosticSeverity.Error,
    isEnabledByDefault: true
  );

  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(ClockUsage);

  public override void Initialize(AnalysisContext context)
  {
    context.EnableConcurrentExecution();
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
  }
}
