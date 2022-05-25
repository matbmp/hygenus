﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class PolygonColliderRenderer : Component, IRenderable
    {
        PolygonCollider polygonCollider;
        VertexPosition[] renderVertices;
        public PolygonColliderRenderer(PolygonCollider polygon)
        {
            this.polygonCollider = polygon;
            renderVertices = new VertexPosition[polygon.polygon.Points.Length];
        }
        public void Render(HyperColorEffect effect)
        {
            effect.ObjectTranslation = Vector3.Zero;
            effect.ObjectRotation = Quaternion.Identity;
            effect.Color = Color.Red.ToVector4();
            effect.CurrentTechnique.Passes[0].Apply();
            for(int i = 0; i < polygonCollider.WorldPoints.Length; i++)
            {
                renderVertices[i] = new VertexPosition(new Vector3(polygonCollider.WorldPoints[PolygonRenderer.stripIndex(i, polygonCollider.WorldPoints.Length)], 0.0F));
            }
            effect.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, renderVertices, 0, renderVertices.Length-3);
        }
    }
}