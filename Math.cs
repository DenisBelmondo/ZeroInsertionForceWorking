using System.Numerics;

namespace Belmondo;

public static class Math
{
    public static float Lerp(float from, float to, float t) => (1 - t) * from + t * to;
    public static double Lerp(double from, double to, double t) => (1 - t) * from + t * to;
    public static float Stepify(float value, float step) => (float)MathF.Floor(value / step) * step;
    public static double Stepify(double value, double step) => System.Math.Floor(value / step) * step;
    public static Vector2 Lerp(Vector2 from, Vector2 to, float t) => new(Lerp(from.X, to.X, t), Lerp(from.Y, to.Y, t));
    public static Vector3 Lerp(Vector3 from, Vector3 to, float t) => new(Lerp(from.X, to.X, t), Lerp(from.Y, to.Y, t), Lerp(from.Z, to.Z, t));
    public static Vector2 SafeNormalize(Vector2 v) => v.LengthSquared() > 0 ? Vector2.Normalize(v) : Vector2.Zero;
    public static Vector3 SafeNormalize(Vector3 v) => v.LengthSquared() > 0 ? Vector3.Normalize(v) : Vector3.Zero;
    public static Vector2 Stepify(Vector2 v, float step) => new(Stepify(v.X, step), Stepify(v.Y, step));
    public static Vector3 Stepify(Vector3 v, float step) => new(Stepify(v.X, step), Stepify(v.Y, step), Stepify(v.Z, step));

    public static float AngleBetween(Vector2 v1, Vector2 v2)
    {
        float dot = v1.X * v2.X + v1.Y * v2.Y;
        float det = v1.X * v2.Y - v1.Y * v2.X;

        return MathF.Atan2(det, dot);
    }
}

public static class MathExtensions
{
    public static Vector2 Lerp(this Vector2 from, Vector2 to, float t) => Math.Lerp(from, to, t);
    public static Vector3 Lerp(this Vector3 from, Vector3 to, float t) => Math.Lerp(from, to, t);
    public static Vector2 SafeNormalize(this Vector2 v) => Math.SafeNormalize(v);
    public static Vector3 SafeNormalize(this Vector3 v) => Math.SafeNormalize(v);
    public static float AngleTo(this Vector2 v1, Vector2 v2) => Math.AngleBetween(v1, v2);
    public static Vector2 Stepify(this Vector2 v, float step) => new(Math.Stepify(v.X, step), Math.Stepify(v.Y, step));
    public static Vector3 Stepify(this Vector3 v, float step) => new(Math.Stepify(v.X, step), Math.Stepify(v.Y, step), Math.Stepify(v.Z, step));
}
