using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class CollisionResult
    {
        public Vector2[] contacts;
        public Vector2 Normal;
        public float penetration;
        public PolygonCollider penetrated;
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
                    Vector2 ra = contacts[i] - penetrated.transformation.Translation;
                    Vector2 rb = contacts[i] - penetrating.transformation.Translation;

                    // Relative velocity
                    Vector2 rv = penetrating.velocity + Math2d.Cross(penetrating.angularVelocity, rb) -
                        penetrated.velocity - Math2d.Cross(penetrated.angularVelocity, ra);

                    float contactVel = Vector2.Dot(rv, Normal);
                    float raCrossN = Math2d.Cross(ra, Normal);
                    float rbCrossN = Math2d.Cross(rb, Normal);

                    float j = contactVel;

                    if (float.IsNaN(j))
                    {
                        throw new Exception("impulse scalar is NaN");
                    }

                    Vector2 impulse = Normal * j;
                    impulse *= -0.2F;
                    penetrated.ApplyImpulse(-impulse, ra);
                    penetrating.ApplyImpulse(impulse, rb);
                }
            }
            
        }
    }
}
