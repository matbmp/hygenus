using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Hygenus
{
    [DataContract]
    public class GyroVector
    {
        public static GyroVector IDENTITY = new GyroVector(0, 0, 0);
        public static float K = HyperMath.K;

        [DataMember]
        public Vector3 vec;
        [DataMember]
        public Quaternion gyr = Quaternion.Identity;

        public GyroVector(float x, float y, float z)
        {
            vec = new Vector3(x, y, z);
        }
        public GyroVector(Vector2 vector)
        {
            vec = new Vector3(vector.X, vector.Y, 0.0F);
        }
        public GyroVector(Vector3 v)
        {
            vec = v;
        }
        public GyroVector(Vector3 v, Quaternion q)
        {
            vec = v;
            gyr = q;
        }
        public static explicit operator Vector2(GyroVector gv)
        {
            return new Vector2(gv.vec.X, gv.vec.Y);
        }
        public static bool operator ==(GyroVector value1, GyroVector value2)
        {
            return value1.vec == value2.vec && value1.gyr == value2.gyr;
        }
        public static bool operator !=(GyroVector value1, GyroVector value2)
        {
            return !(value1 == value2);
        }

        public static GyroVector operator -(GyroVector gv)
        {
            return new GyroVector(-Vector3.Transform(gv.vec, gv.gyr), Quaternion.Inverse(gv.gyr));
        }
        public static GyroVector operator -(GyroVector a, GyroVector b)
        {
            return a + (-b);
        }

        public static GyroVector operator +(GyroVector gv1, GyroVector gv2)
        {
            
            HyperMath.MobiusAddGyr(gv1.vec, Vector3.Transform(gv2.vec, Quaternion.Inverse(gv1.gyr)), out Vector3 newVec, out Quaternion newGyr);
            return new GyroVector(newVec, gv2.gyr * gv1.gyr * newGyr);
            //Vector3D newVec = MobiusAddition(gv1.vec, rotateVector(QuaternionD.Inverse(gv1.gyr), gv2.vec));
            //QuaternionD newGyr = MobiusGyr(gv1.vec, rotateVector(QuaternionD.Inverse(gv1.gyr), gv2.vec));
            //newGyr = QuaternionD.identity;
            //return new GyroVectorD(newVec, gv2.gyr * gv1.gyr * newGyr);
        }

        public static GyroVector operator *(GyroVector a, float r)
        {
            float l = (float)a.vec.Length();
            if (r == 0 || l == 0) return new GyroVector(0.0F, 0.0F, 0.0F);
            float plus = (float)Math.Pow((1 - K * l), r);
            float minus = (float)Math.Pow((1 + K * l), r);
            float m = (-K * ((plus - minus) / (plus + minus)) / l);
            return new GyroVector(a.vec * m);
        }
        public static GyroVector operator /(GyroVector a, float r)
        {
            return (a * (1 / r));
        }

        public GyroVector rotated(float phi)
        {
            return new GyroVector(Vector3.Transform(vec, Quaternion.CreateFromYawPitchRoll(0.0F, 0.0F, phi)));
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
    }

    public struct Vector3D
    {
        public Vector3D(double _x, double _y, double _z) { X = _x; Y = _y; Z = _z; }
        public static readonly Vector3D zero = new Vector3D(0.0, 0.0, 0.0);
        public static readonly Vector3D up = new Vector3D(0.0, 1.0, 0.0);

        public double sqrMagnitude
        {
            get { return X * X + Y * Y + Z * Z; }
        }
        public Vector3D normalized
        {
            get { return this / Math.Sqrt(sqrMagnitude); }
        }

        public static double Dot(Vector3D a, Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        public static Vector3D Cross(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }
        public static Vector3D Project(Vector3D v, Vector3D n)
        {
            return n * (Dot(v, n) / n.sqrMagnitude);
        }
        public static Vector3D Lerp(Vector3D a, Vector3D b, double t)
        {
            return a * (1.0 - t) + b * t;
        }
        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Vector3D operator -(Vector3D a)
        {
            return new Vector3D(-a.X, -a.Y, -a.Z);
        }
        public static Vector3D operator *(Vector3D v, double d)
        {
            return new Vector3D(v.X * d, v.Y * d, v.Z * d);
        }
        public static Vector3D operator *(double d, Vector3D v)
        {
            return new Vector3D(v.X * d, v.Y * d, v.Z * d);
        }
        public static Vector3D operator /(Vector3D v, double d)
        {
            return new Vector3D(v.X / d, v.Y / d, v.Z / d);
        }
        public static explicit operator Vector3(Vector3D v)
        {
            return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        }
        public static explicit operator Vector3D(Vector3 v)
        {
            return new Vector3D(v.X, v.Y, v.Z);
        }

        public double X;
        public double Y;
        public double Z;
    }

    public struct QuaternionD
    {
        public QuaternionD(double _x, double _y, double _z, double _w) { X = _x; Y = _y; Z = _z; W = _w; }
        public static readonly QuaternionD identity = new QuaternionD(0.0, 0.0, 0.0, 1.0);

        public QuaternionD normalized
        {
            get
            {
                double invMag = 1.0 / Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
                return new QuaternionD(invMag * X, invMag * Y, invMag * Z, invMag * W);
            }
        }
        public void Normalize()
        {
            double invMag = 1.0 / Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
            X *= invMag; Y *= invMag; Z *= invMag; W *= invMag;
        }

        public static QuaternionD Inverse(QuaternionD q)
        {
            return new QuaternionD(-q.X, -q.Y, -q.Z, q.W);
        }
        public static QuaternionD Lerp(QuaternionD a, QuaternionD b, double t)
        {
            double t2 = t * Math.Sign(a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W);
            double x = a.X * (1.0 - t) + b.X * t2;
            double y = a.Y * (1.0 - t) + b.Y * t2;
            double z = a.Z * (1.0 - t) + b.Z * t2;
            double w = a.W * (1.0 - t) + b.W * t2;
            return new QuaternionD(x, y, z, w).normalized;
        }
        public static QuaternionD FromToRotation(Vector3D a, Vector3D b)
        {
            Vector3D c = Vector3D.Cross(a, b);
            double w = Math.Sqrt(a.sqrMagnitude * b.sqrMagnitude) + Vector3D.Dot(a, b);
            return new QuaternionD(c.X, c.Y, c.Z, w).normalized;
        }
        public static QuaternionD operator *(QuaternionD q, QuaternionD r)
        {
            return new QuaternionD(
                r.W * q.X + r.X * q.W - r.Y * q.Z + r.Z * q.Y,
                r.W * q.Y + r.X * q.Z + r.Y * q.W - r.Z * q.X,
                r.W * q.Z - r.X * q.Y + r.Y * q.X + r.Z * q.W,
                r.W * q.W - r.X * q.X - r.Y * q.Y - r.Z * q.Z).normalized;
        }
        public static Vector3D operator *(QuaternionD q, Vector3D v)
        {
            Vector3D r = new Vector3D(q.X, q.Y, q.Z);
            return v + Vector3D.Cross(2.0 * r, Vector3D.Cross(r, v) + q.W * v);
        }
        public static explicit operator Quaternion(QuaternionD v)
        {
            return new Quaternion((float)v.X, (float)v.Y, (float)v.Z, (float)v.W);
        }
        public static explicit operator QuaternionD(Quaternion v)
        {
            return new QuaternionD(v.X, v.Y, v.Z, v.W);
        }

        public double X;
        public double Y;
        public double Z;
        public double W;
    }
    class GyroVectorD
    {
        private static readonly float u = 0.01F;
        public static GyroVectorD IDENTITY = new GyroVectorD(0, 0, 0);
        public static GyroVectorD UNITX = new GyroVectorD(u, 0.0F, 0.0F);
        public static GyroVectorD UNITY = new GyroVectorD(0.0F, u, 0.0F);
        public static GyroVectorD UNITZ = new GyroVectorD(0.0F, 0.0F, u);
        public static float K = 1.0F;
        public Vector3D vec;
        public QuaternionD gyr = QuaternionD.identity;

        public GyroVectorD(double x, double y, double z)
        {
            vec = new Vector3D(x, y, z);
        }
        public GyroVectorD(Vector3D v)
        {
            vec = v;
        }
        public GyroVectorD(Vector3D v, QuaternionD q)
        {
            vec = v;
            gyr = q;
        }

        public static GyroVectorD operator -(GyroVectorD gv)
        {
            return new GyroVectorD(-rotateVector(gv.gyr, gv.vec), QuaternionD.Inverse(gv.gyr));
        }
        public static GyroVectorD operator -(GyroVectorD a, GyroVectorD b)
        {
            return a + (-b);
        }
        public static explicit operator GyroVector(GyroVectorD gv)
        {
            return new GyroVector(new Vector3((float)gv.vec.X, (float)gv.vec.Y, (float)gv.vec.Z), new Quaternion((float)gv.gyr.X, (float)gv.gyr.Y, (float)gv.gyr.Z, (float)gv.gyr.W));
        }


        public static GyroVectorD operator +(GyroVectorD gv1, GyroVectorD gv2)
        {
            MobiusAddGyr(gv1.vec, QuaternionD.Inverse(gv1.gyr) * gv2.vec, out Vector3D newVec, out QuaternionD newGyr);
            return new GyroVectorD(newVec, gv2.gyr * gv1.gyr * newGyr);
            //Vector3D newVec = MobiusAddition(gv1.vec, rotateVector(QuaternionD.Inverse(gv1.gyr), gv2.vec));
            //QuaternionD newGyr = MobiusGyr(gv1.vec, rotateVector(QuaternionD.Inverse(gv1.gyr), gv2.vec));
            //newGyr = QuaternionD.identity;
            //return new GyroVectorD(newVec, gv2.gyr * gv1.gyr * newGyr);
        }
        public static void MobiusAddGyrUnnorm(Vector3D a, Vector3D b, out Vector3D sum, out QuaternionD gyr)
        {
            Vector3D c = (-K) * Vector3D.Cross(a, b);
            double d = 1.0f - (-K) * Vector3D.Dot(a, b);
            Vector3D t = a + b;
            sum = (t * d + Vector3D.Cross(c, t)) / (d * d + c.sqrMagnitude);
            gyr = new QuaternionD(-c.X, -c.Y, -c.Z, d);
        }
        public static void MobiusAddGyr(Vector3D a, Vector3D b, out Vector3D sum, out QuaternionD gyr)
        {
            MobiusAddGyrUnnorm(a, b, out sum, out gyr);
            gyr.Normalize();
        }
        public static Vector3D rotateVector(QuaternionD q, Vector3D v)
        {
            // Extract the vector part of the quaternion
            Vector3D u = new Vector3D(q.X, q.Y, q.Z);

            // Extract the scalar part of the quaternion
            double s = q.W;

            // Do the math
            return 2.0f * Vector3D.Dot(u, v) * u
              + (s * s - Vector3D.Dot(u, u)) * v
              + 2.0f * s * Vector3D.Cross(u, v);
        }
        public static Vector3D MobiusAddition(Vector3D a, Vector3D b)
        {
            Vector3D c = (-K) * Vector3D.Cross(a, b);
            double d = 1.0f - (-K) * Vector3D.Dot(a, b);
            Vector3D t = a + b;
            return (t * d + Vector3D.Cross(c, t)) / (d * d + c.sqrMagnitude);
            // EQUIVALENT
            //return (((1 + (2 * K * Vector3D.Dot(a, b)) + K * Vector3D.Dot(b, b)) * a + (1 - K * Vector3D.Dot(a, a)) * b)
            //    / (1 + 2 * K * Vector3D.Dot(a, b) + K * K * Vector3D.Dot(a, a) * Vector3D.Dot(b, b)));
        }
        public static GyroVectorD simult(GyroVectorD a, float r)
        {
            return new GyroVectorD(a.vec * r);
        }
        public static GyroVectorD operator *(GyroVectorD a, float r)
        {
            float l = (float)a.Length();
            if (r == 0 || l == 0) return new GyroVectorD(0.0F, 0.0F, 0.0F);
            float plus = (float)Math.Pow((1 + K * l), r);
            float minus = (float)Math.Pow((1 - K * l), r);
            float m = (K * ((plus - minus) / (plus + minus)) / l);
            return new GyroVectorD(a.vec * m);
        }
        public static double operator *(GyroVectorD a, GyroVectorD b)
        {
            return a.vec.X * b.vec.X + a.vec.Y * b.vec.Y + a.vec.Z * b.vec.Z;
        }
        public static GyroVectorD operator /(GyroVectorD a, float r)
        {
            return (a * (1 / r));
        }

        public GyroVectorD rotated(Quaternion q)
        {
            return new GyroVectorD(rotateVector((QuaternionD)q, vec), gyr);
        }
        public GyroVectorD rotated(float phi)
        {
            return rotated(Quaternion.CreateFromYawPitchRoll(0.0F, 0.0F, phi));
        }


        public static float PythagoreanLength(GyroVectorD a, GyroVectorD b)
        {
            return (float)Math.Sqrt((a.normSq() + b.normSq()) / (1 + a.normSq() * b.normSq()));
        }
        //3D Möbius gyration
        public static QuaternionD MobiusGyr(Vector3D a, Vector3D b)
        {
            //We're actually doing this operation:
            //  Quaternion.AngleAxis(180.0f, MobiusAdd(a, b)) * Quaternion.AngleAxis(180.0f, a + b);
            //But the precision is better (and faster) by doing the way below:
            Vector3D c = K * Vector3D.Cross(a, b);
            double d = 1.0f - K * Vector3D.Dot(a, b);
            QuaternionD q = new QuaternionD(c.X, c.Y, c.Z, d);
            q.Normalize();
            return q;
        }



        public double normSq()
        {
            return this * this;
        }
        public float Length()
        {
            return (float)Math.Sqrt(this * this);
        }
        public GyroVectorD Normalized()
        {
            return this / Length();
        }
        public static double cosine(GyroVectorD a, GyroVectorD b)
        {
            return (a * b) / (a.Length() * b.Length());
        }
        public GyroVectorD Copy()
        {
            return new GyroVectorD(vec.X, vec.Y, vec.Z);
        }

    }
}
