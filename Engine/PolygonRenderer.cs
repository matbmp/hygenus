using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class PolygonRenderer : Component, IRenderable
    {
        private Transformation transformation;
        private Polygon polygon;
        private VertexPosition[] renderVertices;
        private Color PolygonColor { get; set; }
        public PolygonRenderer(PolygonCollider polygonCollider)
        {
            transformation = polygonCollider.transformation;
            polygon = polygonCollider.polygon;
            renderVertices = new VertexPosition[polygon.Points.Length];
            for(int i = 0; i < polygon.Points.Length; i++)
            {
                renderVertices[i] = new VertexPosition(new Microsoft.Xna.Framework.Vector3(polygon.Points[stripIndex(i, polygon.Points.Length)], 0.0F));
            }
            PolygonColor = Color.Aqua;
        }
        public PolygonRenderer(Transformation transformation, Polygon polygon, Color polygonColor)
        {
            this.transformation = transformation;
            this.polygon = polygon;
            PolygonColor = polygonColor;
            renderVertices = new VertexPosition[polygon.Points.Length];
            for (int i = 0; i < polygon.Points.Length; i++)
            {
                renderVertices[i] = new VertexPosition(new Microsoft.Xna.Framework.Vector3(polygon.Points[stripIndex(i, polygon.Points.Length)], 0.0F));
            }
        }

        public void Render(HyperColorEffect effect)
        {
            effect.Color = PolygonColor.ToVector4();
            effect.ObjectTranslation = new Vector3(transformation.Translation, 0.0F);
            effect.ObjectRotation = transformation.Rotation;
            effect.CurrentTechnique.Passes[0].Apply();
            effect.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, renderVertices, 0, renderVertices.Length-2);
        }

        public static int stripIndex(int i, int len)
        {
            return i < 2 ? i : ((i % 2 == 0) ? len - i / 2 : (i + 1) / 2);
        }
    }
}
