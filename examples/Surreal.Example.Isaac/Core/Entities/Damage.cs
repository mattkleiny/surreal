namespace Isaac.Core.Entities {
  public readonly struct Damage {
    public Damage(int amount, DamageType type = DamageType.Physical) {
      Amount = amount;
      Type   = type;
    }

    public readonly int        Amount;
    public readonly DamageType Type;

    public override string ToString() => $"<{Amount} {Type}>";
  }
}