using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;

namespace Hygenus
{
    public class FinishLine : Entity
    {
        private int lastRedTick = int.MinValue;
        HyperPolygonCollider frontLine;
        ColoredPolygonRenderer frontCPR;
        public FinishLine() : base("FinishLine")
        {
            float width, thickness;
            width = 0.6F;
            thickness = 0.6F;
            frontLine = new HyperPolygonCollider(Figures.Quad(width, thickness));
            frontCPR = new ColoredPolygonRenderer(frontLine.localTransformation, frontLine.polygon, Color.Beige);
            frontLine.isStatic = true;
            frontLine.collisionResolution = false;
            frontLine.OnCollided = delegate (PolygonCollider other)
            {
                if(other.Entity is PlayerEntity && other.collisionResolution)
                {
                    lastRedTick = tick;
                }
            };

            HyperPolygonCollider backLine = new HyperPolygonCollider(Figures.Quad(width, thickness));
            backLine.localTransformation.Translation -= new Vector2(0.0F, thickness);
            backLine.isStatic = true;
            backLine.collisionResolution = false;

            this.AddComponent(frontLine);
            this.AddComponent(frontCPR);
        }

        public override void Update()
        {
            base.Update();
            if(lastRedTick + 3 < tick)
            {
                frontCPR.PolygonColor = Color.Beige;
            }
            else
            {
                frontCPR.PolygonColor = Color.Red;
            }
        }
    }
}
