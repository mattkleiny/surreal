namespace Asteroids.Actors;

internal readonly record struct PlayerHitAsteroid(Player Player, Asteroid Asteroid);
internal readonly record struct ProjectileHitAsteroid(Projectile Projectile, Asteroid Asteroid);
