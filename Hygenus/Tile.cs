using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hygenus
{
    class TileMap
    {
        public static List<GyroVector> createTiles(int n, int m, out Polygon p)
        {
            int i, j, k;
            float q = (float)(Math.PI / 4);
            double rr = Math.Sqrt(Math.Cos(Math.PI / n + Math.PI / 4) / Math.Cos(Math.PI / n - Math.PI / 4));  //(The AAA to SSS Conversion Theorem)
            GyroVectorD x= new GyroVectorD((float)rr, 0.0F, 0.0F);
            GyroVectorD mid = x.rotated(q) + ((-(x.rotated(q)) + (x.rotated(7 * q))) * 0.5F);
            float rrr = (mid * 2).Length();

            GyroVectorD check;
            Queue<GyroVectorD> toCheck = new Queue<GyroVectorD>();
            List<GyroVectorD> done = new List<GyroVectorD>();
            toCheck.Enqueue(new GyroVectorD(0D, 0D, 0D));
            for (i = 0; i < m; i++)
            {
                k = toCheck.Count;
                for (j = 0; j < k; j++)
                {
                    check = toCheck.Dequeue();
                    tryAdd(done, check);
                    toCheck.Enqueue(new GyroVectorD((float)rrr, 0.0F, 0.0F) + check);
                    toCheck.Enqueue(new GyroVectorD((float)-rrr, 0.0F, 0.0F) + check);
                    toCheck.Enqueue(new GyroVectorD(0.0F, (float)rrr, 0.0F) + check);
                    toCheck.Enqueue(new GyroVectorD(0.0F, (float)-rrr, 0.0F) + check);
                }
            }
            List<GyroVector> result = new List<GyroVector>();
            for (i = 0; i < done.Count; i++)
            {
                result.Add((GyroVector)done[i]);
            }

            Random rand = new Random();
            GyroVector gv = new GyroVector((float)rr, 0.0F);
            List<ColoredPolygonRenderer> tiles = new List<ColoredPolygonRenderer>();
            Vector2[] tile = new Vector2[4];
            Vector2 a = gv.rotated(q).vec, b = gv.rotated(3 * q).vec, c = gv.rotated(5 * q).vec, d = gv.rotated(7 * q).vec;
            tile[0] = new Vector2(a.X, a.Y);
            tile[1] = new Vector2(b.X, b.Y);
            tile[2] = new Vector2(c.X, c.Y);
            tile[3] = new Vector2(d.X, d.Y);
            p = new Polygon(tile);

            /*
            foreach (GyroVector v in result)
            {
                float cc = (float)rand.NextDouble();
                tiles.Add(new ColoredPolygonRenderer(new Transformation(new Vector2(v.vec.X, v.vec.Y), (v.gyr)), p, new Color((uint)rand.Next())));
            }
            */
            return result;
        }
        private static bool tryAdd(List<GyroVectorD> l, GyroVectorD a)
        {
            foreach (GyroVectorD v in l)
            {
                if ((a - v).normSq() < 0.01F)
                {
                    return false;
                }
            }
            l.Add(a);
            return true;
        }
    }
}
