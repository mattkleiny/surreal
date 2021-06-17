﻿using System;

namespace Surreal {
  public readonly struct Unit : IEquatable<Unit> {
    public static Unit Default => default;

    public override int  GetHashCode()      => 0;
    public          bool Equals(Unit other) => true;

    public override string ToString() => "()";
  }
}