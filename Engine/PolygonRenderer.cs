using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    /// <summary>
    /// Klasa rysująca wielokąt, jeżeli do konstruktora podano PolygonCollider, to localTransformation jest dzielone między
    /// colliderem a nowo utworzonym rendererem
    /// </summary>
    [DataContract]
    public class PolygonRenderer : Component, IRenderable
    {
        [DataMember]
        private VertexPosition[] renderVertices;
        [DataMember]
        public Color PolygonColor { get; set; }
        
        public PolygonRenderer(PolygonCollider polygonCollider): base()
        {
            localTransformation = polygonCollider.localTransformation;
            Polygon polygon = polygonCollider.polygon;
            renderVertices = new VertexPosition[polygon.Points.Length];
            for(int i = 0; i < polygon.Points.Length; i++)
            {
                renderVertices[i] = new VertexPosition(new Vector3(polygon.Points[stripIndex(i, polygon.Points.Length)], 0.0F));
            }
            PolygonColor = Color.Aqua;
        }
        public PolygonRenderer(Transformation transformation, Polygon polygon, Color polygonColor)
        {
            this.localTransformation = transformation;
            PolygonColor = polygonColor;
            renderVertices = new VertexPosition[polygon.Points.Length];
            for (int i = 0; i < polygon.Points.Length; i++)
            {
                renderVertices[i] = new VertexPosition(new Vector3(polygon.Points[stripIndex(i, polygon.Points.Length)], 0.0F));
            }
        }

        public void Render(HyperColorEffect effect)
        {
            Transformer transformer = Entity.scene.Transformer;
            Transformation final = transformer.Combine(Entity.transformation, localTransformation);
            effect.Color = PolygonColor.ToVector4();
            effect.ObjectTranslation = new Vector3(final.Translation, 0.0F);
            effect.ObjectRotation = final.Rotation;
            effect.ObjectScale = new Vector3(localTransformation.Scale, 0F);
            effect.CurrentTechnique.Passes[0].Apply();
            effect.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, renderVertices, 0, renderVertices.Length-2);
        }

        public static int stripIndex(int i, int len)
        {
            return i < 2 ? i : ((i % 2 == 0) ? len - i / 2 : (i + 1) / 2);
        }

        public void updateScale(Polygon p, Vector2 scale)
        {
            renderVertices = new VertexPosition[p.Points.Length];
            for (int i = 0; i < p.Points.Length; i++)
            {
                renderVertices[i] = new VertexPosition(new Microsoft.Xna.Framework.Vector3(p.Points[stripIndex(i, p.Points.Length)], 0.0F));
            }
        }
    }
}
