using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;

namespace Hygenus
{
    public class HyperPolygonCollider : PolygonCollider
    {
        public HyperPolygonCollider()
        {

        }
        public HyperPolygonCollider(params Vector2[] points) : base(points)
        {
        }
        public override void Update()
        {
            UpdateWorldTransformation();
            Vector2[] originalPoints = polygon.Points;
            GyroVector gv, gv2;
            gv = new GyroVector(WorldTransformation.Translation, WorldTransformation.Gyration * WorldTransformation.Rotation);
            for (int i = 0; i < originalPoints.Length; i++)
            {
                gv2 = gv + new GyroVector(originalPoints[i] * localTransformation.Scale);
                WorldPoints[i] = new Vector2(gv2.vec.X, gv2.vec.Y);
                WorldPoints[i] = HyperMath.PoincareToKlein(WorldPoints[i]) * 1.0F;
            }
            for (int i = 0; i < WorldPoints.Length; i++)
            {
                Vector2 face = WorldPoints[(i + 1) % WorldPoints.Length] - WorldPoints[i];
                WorldEdgeNormals[i] = new Vector2(face.Y, -face.X);
                WorldEdgeNormals[i].Normalize();
            }
        }
        public override void OnAddedToEntity()
        {
            
        }

        public override void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
        {
            Entity.ApplyImpulse2(impulse, contactVector);
        }
    }
}
