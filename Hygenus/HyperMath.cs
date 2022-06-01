using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Hygenus
{
    internal class HyperMath
    {
        // K - zakrzywienie przestrzeni (-1 to hyperboliczna)
        public static float K = -1.0F;
        public static void MobiusAddGyrUnnorm(Vector3 a, Vector3 b, out Vector3 vec, out Quaternion gyr)
        {
            Vector3 c = K * Vector3.Cross(a, b);
            float d = 1.0f - K * Vector3.Dot(a, b);
            Vector3 t = a + b;
            vec = (t * d + Vector3.Cross(c, t)) / (d * d + c.LengthSquared());
            gyr = new Quaternion(-c.X, -c.Y, -c.Z, d);
        }
        public static void MobiusAddGyr(Vector3 a, Vector3 b, out Vector3 sum, out Quaternion gyr)
        {
            MobiusAddGyrUnnorm(a, b, out sum, out gyr);
            gyr.Normalize();
        }

        public static float MobiusDistSq(Vector3 a, Vector3 b)
        {
            float a2 = a.LengthSquared();
            float b2 = b.LengthSquared();
            float ab = 2.0f * Vector3.Dot(a, b);
            return (a2 - ab + b2) / (1.0f + K * (ab + K * a2 * b2));
        }

        //3D Möbius gyration
        public static Quaternion MobiusGyr(Vector3 a, Vector3 b)
        {
            //We're actually doing this operation:
            //  Quaternion.AngleAxis(180.0f, MobiusAdd(a, b)) * Quaternion.AngleAxis(180.0f, a + b);
            //But the precision is better (and faster) by doing the way below:
            Vector3 c = -K * Vector3.Cross(a, b);
            float d = 1.0f + K * Vector3.Dot(a, b);
            Quaternion q = new Quaternion(c.X, c.Y, c.Z, d);
            q.Normalize();
            return q;
        }
        public static Vector3 gyration(Vector3 a, Vector3 b, Vector3 z)
        {
            return -MobiusAddition(a, b) + MobiusAddition(a, MobiusAddition(b, z));
        }

        public static Vector3 MobiusAddition(Vector3 a, Vector3 b)
        {
            Vector3 c = (-K) * Vector3.Cross(a, b);
            float d = 1.0f - (-K) * Vector3.Dot(a, b);
            Vector3 t = a + b;
            return (t * d + Vector3.Cross(c, t)) / (d * d + c.LengthSquared());
            // EQUIVALENT
            //return (((1 + (2 * K * Vector3D.Dot(a, b)) + K * Vector3D.Dot(b, b)) * a + (1 - K * Vector3D.Dot(a, a)) * b)
            //    / (1 + 2 * K * Vector3D.Dot(a, b) + K * K * Vector3D.Dot(a, a) * Vector3D.Dot(b, b)));
        }
        public static Vector2 MobiusAddition(Vector2 a, Vector2 b)
        {

            float c = (-K) * Cross(a, b);
            float d = 1.0f - (-K) * Vector2.Dot(a, b);
            Vector2 t = a + b;
            return (t * d + Cross(c, t)) / (d * d + c);
            // EQUIVALENT
            //return (((1 + (2 * K * Vector3D.Dot(a, b)) + K * Vector3D.Dot(b, b)) * a + (1 - K * Vector3D.Dot(a, a)) * b)
            //    / (1 + 2 * K * Vector3D.Dot(a, b) + K * K * Vector3D.Dot(a, a) * Vector3D.Dot(b, b)));
        }

        public static float MobiusMultiply(float x, float r)
        {
            if (x == 0 || r == 0) return x;
            float plus = (float)Math.Pow((1 - K * x), r);
            float minus = (float)Math.Pow((1 + K * x), r);
            float m = (-K * ((plus - minus) / (plus + minus)) / x);
            return (x * m);
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
