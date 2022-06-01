using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;

namespace Hygenus
{
    public class HyperbolicDynamicsProvider : DynamicsProvider
    {
        public void ApplyAngularVelocity(Transformation transformation, float angularVelocity)
        {
            transformation.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Backward, angularVelocity);
        }

        public void ApplyVelocity(Transformation transformation, Vector2 velocity)
        {
            GyroVector gv = new GyroVector(new Vector3(transformation.Translation, 0.0F), transformation.Gyration) + new GyroVector(velocity);
            transformation.Translation = new Vector2(gv.vec.X, gv.vec.Y);
            transformation.Gyration = gv.gyr;
        }

        public void ApplyImpulse(Transformation transformation, Vector2 impulse, Vector2 contactVector, ref Vector2 velocity, ref float angularVelocity)
        {
            velocity += impulse;
            angularVelocity += Math2d.Cross(contactVector, impulse);
        }

        public void PositionalCorrection(Transformation transformation, Vector2 impulse)
        {
            transformation.Translation += impulse;
        }
    }
}
