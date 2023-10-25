namespace Surreal.Scripting;

/// <summary>
/// Indicates that a type or member is used by a scripting language.
/// </summary>
[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
public sealed class UsedByScriptAttribute : Attribute;
