namespace BlazorNodes.Core;
using System;

public readonly struct Vector2
{
    public readonly double x;
    public readonly double y;

    public Vector2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public double GetLength()
    {
        return Math.Sqrt(x * x + y * y);
    }

    public static Vector2 operator -(Vector2 v1)
    {
        return new Vector2(-v1.x, -v1.y);
    }

    public static Vector2 operator +(Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.x + v2.x, v1.y + v2.y);
    }

    public static Vector2 operator -(Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.x - v2.x, v1.y - v2.y);
    }

    public static Vector2 operator *(Vector2 v1, double scalar)
    {
        return new Vector2(v1.x * scalar, v1.y * scalar);
    }

    public static Vector2 operator /(Vector2 v1, double scalar)
    {
        return new Vector2(v1.x / scalar, v1.y / scalar);
    }

    public static implicit operator Vector2((double x, double y) input)
    {
        return new Vector2(input.x, input.y);
    }
}
