﻿using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hygenus
{
    public class HyperbolicPolygonColliderRenderer : PolygonColliderRenderer
    {
        public HyperbolicPolygonColliderRenderer(PolygonCollider polygon) : base(polygon)
        {
        }

        public override void Render(HyperColorEffect effect)
        {
            effect.ObjectTranslation = Vector3.Zero;
            effect.ObjectRotation = Quaternion.Identity;
            effect.Color = Color.Red.ToVector4();
            effect.CurrentTechnique.Passes[0].Apply();
            for (int i = 0; i < polygonCollider.WorldPoints.Length; i++)
            {
                renderVertices[i] = new VertexPosition(GyroVector.KleinToPoincare(new Vector3(polygonCollider.WorldPoints[PolygonRenderer.stripIndex(i, polygonCollider.WorldPoints.Length)], 0.0F)));
            }
            effect.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, renderVertices, 0, 1);
        }
    }
}