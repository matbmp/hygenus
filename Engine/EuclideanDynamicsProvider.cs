using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class EuclideanDynamicsProvider : DynamicsProvider
    {
        public void ApplyAngularVelocity(Transformation transformation, float angularVelocity)
        {
            transformation.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Forward, angularVelocity);
        }

        public void ApplyVelocity(Transformation transformation, Vector2 velocity)
        {
            transformation.Translation += velocity;
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
