using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public interface DynamicsProvider
    {
        void ApplyVelocity(Transformation transformation, Vector2 velocity);
        void ApplyAngularVelocity(Transformation transformation, float angularVelocity);
        void ApplyImpulse(Transformation transformation, Vector2 impulse, Vector2 contactVector, ref Vector2 velocity, ref float angularVelocity);
        void PositionalCorrection(Transformation transformation, Vector2 impulse);
    }
}
