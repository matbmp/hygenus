using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    /// <summary>
    /// Klasa przechowujące informacje o wielokącie ze środkiem w punkcie (0,0)
    /// </summary>
    [DataContract]
    public class Polygon
    {
        // punkty wielokąta - środekiem wielokąta musi być punkt (0,0)
        [DataMember]
        public Vector2[] Points { get; set; }
        public Polygon(Vector2[] originalPoints)
        {
            Points = new Vector2[originalPoints.Length];
            Array.Copy(originalPoints, 0, Points, 0, originalPoints.Length);
        }


        /// <summary>
        /// Przesuwa punkty z tablicy tak, aby środek wielokąta znajdował się w punkcie(0, 0)
        /// </summary>
        /// <param name="polygonPoints">Punkty wielokąta</param>
        /// <returns>Dokonane przesunięcie</returns>
        public static Vector2 RecenterPoints(Vector2[] polygonPoints)
        {
            Vector2 center = FindCenter(polygonPoints);
            for(int i = 0; i < polygonPoints.Length; i++)
            {
                polygonPoints[i] -= center;
            }
            return center;
        }
        private static Vector2 FindCenter(Vector2[] polygonPoints)
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
    }
}
