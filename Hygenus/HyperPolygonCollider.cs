using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;

namespace Hygenus
{
    public class HyperPolygonCollider : PolygonCollider
    {
        public HyperPolygonCollider(Vector2[] points) : base(points)
        {
        }
        public override void Update()
        {
            Vector2[] originalPoints = polygon.Points;
            GyroVector gv = new GyroVector(transformation.Translation) + new GyroVector(velocity);
            transformation.Translation = new Vector2(gv.vec.X, gv.vec.Y);
            transformation.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Backward, angularVelocity);
            for (int i = 0; i < originalPoints.Length; i++)
            {
                gv = new GyroVector(transformation.Translation) + new GyroVector(Vector2.Transform(originalPoints[i], Quaternion.Inverse(transformation.Rotation)));
                WorldPoints[i] = new Vector2(gv.vec.X, gv.vec.Y);
                WorldPoints[i] = GyroVector.PoincareToKlein(WorldPoints[i]);
            }
            for (int i = 0; i < WorldPoints.Length; i++)
            {
                Vector2 face = WorldPoints[(i + 1) % WorldPoints.Length] - WorldPoints[i];
                WorldEdgeNormals[i] = new Vector2(face.Y, -face.X);
                WorldEdgeNormals[i].Normalize();
            }
        }
    }
}
