using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class Polygon
    {
        public Vector2[] Points;

        public Polygon(Vector2[] originalPoints)
        {
            Points = new Vector2[originalPoints.Length];
            Array.Copy(originalPoints, 0, Points, 0, originalPoints.Length);
        }

        public static Vector2 FindCenter(Vector2[] polygonPoints)
        {
            Vector2 off = polygonPoints[0];
            float twicearea = 0;
            float x = 0;
            float y = 0;
            Vector2 p1, p2;
            float f;
            for (int i = 0, j = polygonPoints.Length - 1; i < polygonPoints.Length; j = i++)
            {
                p1 = polygonPoints[i];
                p2 = polygonPoints[j];
                f = (p1.X - off.X) * (p2.Y - off.Y) - (p2.X - off.X) * (p1.Y - off.Y);
                twicearea += f;
                x += (p1.X + p2.X - 2 * off.X) * f;
                y += (p1.Y + p2.Y - 2 * off.Y) * f;
            }

            f = twicearea * 3;
            return new Vector2(x / f + off.X, y / f + off.Y);
        }
        public static Vector2 RecenterPoints(Vector2[] polygonPoints)
        {
            Vector2 center = FindCenter(polygonPoints);
            for(int i = 0; i < polygonPoints.Length; i++)
            {
                polygonPoints[i] -= center;
            }
            return center;
        }
    }
}
