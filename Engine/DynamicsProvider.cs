using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    /// <summary>
    /// Interfejs określający w jaki sposób wykonywać dynamikę ruchu tzn. w jaki sposób aktualizować transformację przy użyciu prędkości,
    /// prędkości obrotowej, w jaki sposób zmienić prędkość przy przyłożeniu siły, oraz jak zapobiegać przenikaniu się obiektów(PositionalCorrection)
    /// </summary>
    public interface DynamicsProvider
    {
        void ApplyVelocity(Transformation transformation, Vector2 velocity);
        void ApplyAngularVelocity(Transformation transformation, float angularVelocity);
        void ApplyImpulse(Transformation transformation, Vector2 impulse, Vector2 contactVector, ref Vector2 velocity, ref float angularVelocity);
        void PositionalCorrection(Transformation transformation, Vector2 correction);
    }
}
