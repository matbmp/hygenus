using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;

namespace Hygenus
{
    public class FinishLine : Entity
    {
        public FinishLine() : base("FinishLine")
        {
            float width, thickness;
            width = 0.4F;
            thickness = 0.05F;
            HyperPolygonCollider frontLine = new HyperPolygonCollider(Figures.Quad(width, thickness));
            frontLine.isStatic = true;
            HyperPolygonCollider backLine = new HyperPolygonCollider(Figures.Quad(width, thickness));
            backLine.localTransformation.Translation -= new Vector2(0.0F, thickness);
            backLine.isStatic = true;

            this.AddComponent(frontLine);
            //this.AddComponent(backLine);

            PolygonRenderer frontRender = new PolygonRenderer(frontLine);
            frontRender.PolygonColor = Color.Red;
            PolygonRenderer backRender = new PolygonRenderer(backLine);
            backRender.PolygonColor = Color.RoyalBlue;

            this.AddComponent(frontRender);
            //this.AddComponent(backRender);
        }
    }
}
