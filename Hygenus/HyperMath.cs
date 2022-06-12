using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Hygenus
{
    public static class HyperMath
    {
        // K - zakrzywienie przestrzeni (-1 to hyperboliczna)
        public static float K = -1F;

        /// <summary>
        /// Dodawanie Mobiusa w kuli (Ugnar, AHG, Def. 3.38, s.75)
        /// </summary>
        /// <param name="a">pierwszy wektor</param>
        /// <param name="b">drugi wektor</param>
        /// <returns></returns>
        public static Vector2 MobiusAddition(Vector2 a, Vector2 b)
        {
            float K = -HyperMath.K;
            float adot = Vector2.Dot(a, a), bdot = Vector2.Dot(b, b), abdot2 = 2*Vector2.Dot(a,b);
            float x = (1 + K * (abdot2 + bdot));
            float y = (1 - K * adot);
            float z = (1 + K * (abdot2 + K * adot*bdot));
            return (x * a + y * b) / z;
        }

        /// <summary>
        /// Gyrator, operator naprawiający przemienność oraz łączność dodawania mobiusa (Ugnar, AHG, s.73)
        /// </summary>
        /// <param name="a">pierwszy wektor</param>
        /// <param name="b">drugi wektor</param>
        /// <returns></returns>
        public static Quaternion Gyration(Vector2 a, Vector2 b)
        {
            Quaternion result = new Quaternion(0, 0, -K * Cross(a, b), 1.0f - K * Vector2.Dot(a, b));
            result.Normalize();
            return result;
        }

        public static Vector3 KleinToPoincare(Vector3 p)
        {
            if (K == 0.0f) { return p; }
            return p / (MathF.Sqrt(MathF.Max(0.0f, 1.0f + K * p.LengthSquared())) + 1.0f);
        }
        public static Vector2 KleinToPoincare(Vector2 p)
        {
            if (K == 0.0f) { return p; }
            return p / (MathF.Sqrt(MathF.Max(0.0f, 1.0f + K * p.LengthSquared())) + 1.0f);
        }
        public static Vector3 PoincareToKlein(Vector3 p)
        {
            if (K == 0.0f) { return p; }
            return p * 2.0f / (1.0f - K * p.LengthSquared());
        }
        public static Vector2 PoincareToKlein(Vector2 p)
        {
            if (K == 0.0f) { return p; }
            return p * 2.0f / (1.0f - K * p.LengthSquared());
        }

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
    }
}
