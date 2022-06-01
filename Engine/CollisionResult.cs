using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    /// <summary>
    /// Klasa przechowująca informacje o zderzeniach fizycznych wraz z domyślnym sposobem reakcji na takie zdarzenia
    /// </summary>
    public class CollisionResult
    {
        public delegate void CollisionResolution(CollisionResult collisionResult);
        // punkty na wielokącie penetrującym, które kolidują z bokiem wielokąta penetrowanego
        public Vector2[] contacts;
        // wektor normalny ściany która została spenetrowana
        public Vector2 Normal;
        // głębokość penetracji
        public float penetration;
        // wielokąt spenetrowany
        public PolygonCollider penetrated;
        // wielokąt penetrujący
        public PolygonCollider penetrating;

        public CollisionResult(Vector2[] contacts, Vector2 normal, float penetration, PolygonCollider penetrated, PolygonCollider penetrating)
        {
            this.contacts = contacts;
            Normal = normal;
            this.penetration = penetration;
            this.penetrated = penetrated;
            this.penetrating = penetrating;
        }

        public void DefaultResolve()
        {
            
            for(int i = 0; i < contacts.Length; i++)
            {
                if (contacts[i] != null)
                {
                    Vector2 ra = contacts[i] - penetrated.WorldTransformation.Translation;
                    Vector2 rb = contacts[i] - penetrating.WorldTransformation.Translation;

                    // Relative velocity
                    Vector2 rv = penetrating.Entity.velocity + Math2d.Cross(penetrating.Entity.angularVelocity, rb) -
                        penetrated.Entity.velocity - Math2d.Cross(penetrated.Entity.angularVelocity, ra);

                    float contactVel = Vector2.Dot(rv, Normal);
                    float raCrossN = Math2d.Cross(ra, Normal);
                    float rbCrossN = Math2d.Cross(rb, Normal);

                    float j = contactVel;

                    if (float.IsNaN(j))
                    {
                        throw new Exception("impulse scalar is NaN");
                    }

                    Vector2 impulse = Normal * j;
                    impulse *= -0.7F;
                    penetrated.ApplyImpulse(-impulse, ra);
                    penetrating.ApplyImpulse(impulse, rb);

                    Vector2 correction = penetration * Normal * 0.04F;
                    DynamicsProvider dynamics = penetrated.Entity.scene.DynamicsProvider;
                    dynamics.ApplyVelocity(penetrated.Entity.transformation, correction);
                    dynamics.ApplyVelocity(penetrating.Entity.transformation, -correction);

                }
            }
            
        }
    }
}
