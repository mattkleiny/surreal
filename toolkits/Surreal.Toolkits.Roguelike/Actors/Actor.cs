namespace Surreal.Actors;

/// <summary>An actor in the roguelike engine.</summary>
public abstract record Actor;

/// <summary>The player <see cref="Actor"/>.</summary>
public record Hero : Actor;

/// <summary>A monster <see cref="Actor"/>.</summary>
public record Monster : Actor;
