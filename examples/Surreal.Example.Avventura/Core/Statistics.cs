namespace Avventura.Core;

/// <summary>Contains statistics for a character</summary>
public sealed record Statistics
{
  public int Level { get; set; } = 5;

  public int Vigor        { get; init; } = 10;
  public int Mind         { get; init; } = 10;
  public int Endurance    { get; init; } = 10;
  public int Strength     { get; init; } = 10;
  public int Dexterity    { get; init; } = 10;
  public int Intelligence { get; init; } = 10;
  public int Faith        { get; init; } = 10;
  public int Arcane       { get; init; } = 10;

  public int HealthPoints => 100 * Vigor;
  public int FocusPoints  => 100 * Mind;
  public int Stamina      => 5 * Endurance;
  public int Discovery    => 100 + Arcane;
}
