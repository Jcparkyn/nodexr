using System;

namespace Nodexr.Shared
{
    public readonly struct Vector2L
    {
        public readonly long x;
        public readonly long y;

        public Vector2L(long x, long y)
        {
            this.x = x;
            this.y = y;
        }

        public double GetLength()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static Vector2L operator -(Vector2L v1)
        {
            return new Vector2L(-v1.x, -v1.y);
        }
        public static Vector2L operator +(Vector2L v1, Vector2L v2)
        {
            return new Vector2L(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2L operator -(Vector2L v1, Vector2L v2)
        {
            return new Vector2L(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2L operator *(Vector2L v1, double scalar)
        {
            return new Vector2L((long)(v1.x * scalar), (long)(v1.y * scalar));
        }

        public static Vector2L operator /(Vector2L v1, double scalar)
        {
            return new Vector2L((long)(v1.x / scalar), (long)(v1.y / scalar));
        }

        public static implicit operator Vector2L((int x, int y) input)
        {
            return new Vector2L(input.x, input.y);
        }

    }
}
