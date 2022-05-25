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
    internal class Tile
    {
        public VertexPositionColor[] vertices;
        public Color color;

        public Tile(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color)
        {
            this.color = color;
            vertices = new VertexPositionColor[4];
            vertices[0] = new VertexPositionColor(v1, color);
            vertices[1] = new VertexPositionColor(v2, color);
            vertices[2] = new VertexPositionColor(v4, color);
            vertices[3] = new VertexPositionColor(v3, color);
        }
        public Tile translated(GyroVector t)
        {
            return new Tile((t + new GyroVector(vertices[0].Position)).vec,
                (t + new GyroVector(vertices[1].Position)).vec,
                (t + new GyroVector(vertices[2].Position)).vec,
                (t + new GyroVector(vertices[3].Position)).vec,
                color);
        }
        public void setColor(Color c)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = c;
            }
        }
    }

    class TileMap
    {
        public static List<PolygonRenderer> createTiles(int n, int m)
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
            toCheck.Enqueue(GyroVectorD.IDENTITY);
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
            GyroVector gv = new GyroVector((float)rr, 0.0F, 0.0F);
            List<PolygonRenderer> tiles = new List<PolygonRenderer>();
            Vector2[] tile = new Vector2[4];
            Vector3 a = gv.rotated(q).vec, b = gv.rotated(3 * q).vec, c = gv.rotated(5 * q).vec, d = gv.rotated(7 * q).vec;
            tile[0] = new Vector2(a.X, a.Y);
            tile[1] = new Vector2(b.X, b.Y);
            tile[2] = new Vector2(c.X, c.Y);
            tile[3] = new Vector2(d.X, d.Y);
            Polygon p = new Polygon(tile);

            foreach (GyroVector v in result)
            {
                float cc = (float)rand.NextDouble();
                tiles.Add(new PolygonRenderer(new Transformation(new Vector2(v.vec.X, v.vec.Y), v.gyr), p, new Color(cc, cc, cc, 1.0F)));
            }
            return tiles;
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
