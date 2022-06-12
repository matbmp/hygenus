using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Hygenus
{
    /// <summary>
    /// Klasa implementująca żyrowektory patrz - Analytic Hyperbolic Geometry ~ Abraham A. Ungar
    /// </summary>
    [DataContract]
    
    public class GyroVector
    {
        public static float K = HyperMath.K;

        // Pozycja żyrowektora
        [DataMember]
        public Vector2 vec;
        // Rotacja żyrowekotora
        [DataMember]
        public Quaternion gyr = Quaternion.Identity;


        public GyroVector(Vector2 vector)
        {
            vec = vector;
        }
        public GyroVector(float x, float y)
        {
            vec = new Vector2(x, y);
        }
        public GyroVector(Vector2 v, Quaternion q)
        {
            vec = v;
            gyr = q;
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
            return new GyroVector(-Vector2.Transform(gv.vec, gv.gyr), Quaternion.Inverse(gv.gyr));
        }
        public static GyroVector operator -(GyroVector a, GyroVector b)
        {
            return a + (-b);
        }

        public static GyroVector operator +(GyroVector gv1, GyroVector gv2)
        {
            Vector2 tmp = Vector2.Transform(gv2.vec, Quaternion.Inverse(gv1.gyr));
            Vector2 resultVector = HyperMath.MobiusAddition(gv1.vec, tmp);
            Quaternion resultGyr = HyperMath.Gyration(gv1.vec, tmp);
            return new GyroVector(resultVector, gv2.gyr * gv1.gyr * resultGyr);
        }

        public static GyroVector operator *(GyroVector a, float r)
        {
            float l = (float)a.vec.Length();
            if (r == 0 || l == 0) return new GyroVector(0.0F, 0.0F);
            float plus = (float)Math.Pow((1 - K * l), r);
            float minus = (float)Math.Pow((1 + K * l), r);
            float m = (-K * ((plus - minus) / (plus + minus)) / l);
            return new GyroVector(a.vec * m);
        }

        public GyroVector rotated(float phi)
        {
            return new GyroVector(Vector2.Transform(vec, Quaternion.CreateFromYawPitchRoll(0.0F, 0.0F, phi)));
        }
    }

    public struct Vector3D
    {
        public Vector3D(double _x, double _y, double _z) { X = _x; Y = _y; Z = _z; }

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
        public static float K = -HyperMath.K;
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
            return new GyroVector(new Vector2((float)gv.vec.X, (float)gv.vec.Y), new Quaternion((float)gv.gyr.X, (float)gv.gyr.Y, (float)gv.gyr.Z, (float)gv.gyr.W));
        }


        public static GyroVectorD operator +(GyroVectorD gv1, GyroVectorD gv2)
        {
            MobiusAddGyr(gv1.vec, QuaternionD.Inverse(gv1.gyr) * gv2.vec, out Vector3D newVec, out QuaternionD newGyr);
            return new GyroVectorD(newVec, gv2.gyr * gv1.gyr * newGyr);
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

            Vector3D u = new Vector3D(q.X, q.Y, q.Z);

            // Extract the scalar part of the quaternion
            double s = q.W;

            // Do the math
            return 2.0f * Vector3D.Dot(u, v) * u
              + (s * s - Vector3D.Dot(u, u)) * v
              + 2.0f * s * Vector3D.Cross(u, v);
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
        public double normSq()
        {
            return this * this;
        }
        public float Length()
        {
            return (float)Math.Sqrt(this * this);
        }

    }
}
