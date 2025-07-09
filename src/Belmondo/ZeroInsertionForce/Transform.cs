using System.Numerics;

namespace Belmondo.ZeroInsertionForce;

public struct Transform
{
    public Vector2 Position;
    public Vector2 Direction;

    public Transform() => Direction = Vector2.UnitX;
}
