using Microsoft.Xna.Framework;

namespace Engine
{
    public class Math2d
    {
        public static float epsilon = 1.0e-3F;
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }
        public static Vector2 Cross(Vector2 v, float a)
        {
            return new Vector2(a * v.Y, -a * v.X);
        }
        public static Vector2 Cross(float a, Vector2 v)
        {
            return new Vector2(-a * v.Y, a * v.X);
        }
        public static bool BiasGreaterThan(float a, float b)
        {
            const float k_biasRelative = 0.95f;
            const float k_biasAbsolute = 0.01f;
            return a >= b * k_biasRelative + a * k_biasAbsolute;
        }

    }
}
