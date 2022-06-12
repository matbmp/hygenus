using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;
using static Engine.CollisionResult;

namespace Hygenus
{
    public static class HyperbolicCollisionResolver
    {
        public static void Resolve(CollisionResult result)
        {
            Vector2[] contacts = result.contacts;
            PolygonCollider penetrated = result.penetrated;
            PolygonCollider penetrating = result.penetrating;
            Vector2 Normal = result.Normal;
            float penetration = result.penetration;

            // poprawa wektora ormalnego, otrzymaliśmy wektor normalny w modelu Kleina, a potrzebujemy w modelu Poincare
            if (contacts[0] != Vector2.Zero)
            {
                Vector2 KleinTouch = contacts[0] - Normal * penetration;
                Vector2 sideDir = new Vector2(Normal.Y, -Normal.X);

                Vector2 PoincareTouch1 = HyperMath.KleinToPoincare(KleinTouch + sideDir * penetration);
                Vector2 PoincareTouch2 = HyperMath.KleinToPoincare(KleinTouch - sideDir * penetration);
                Vector2 edge = PoincareTouch2 - PoincareTouch1;
                Normal = new Vector2(edge.Y, -edge.X);
                Normal.Normalize();
            }
            else return;
            

            for (int i = 0; i < result.contacts.Length; i++)
            {
                if (contacts[i] != null)
                {
                    
                    Vector2 ra = contacts[i] - HyperMath.PoincareToKlein(penetrated.Entity.transformation.Translation);
                    Vector2 rb = contacts[i] - HyperMath.PoincareToKlein(penetrating.Entity.transformation.Translation);
                    

                    // relative velocity
                    Vector2 rv = Vector2.Transform(penetrating.Entity.velocity, Quaternion.Inverse(penetrating.Entity.transformation.Gyration)) + Math2d.Cross(penetrating.Entity.angularVelocity, rb) -
                        Vector2.Transform(penetrated.Entity.velocity, Quaternion.Inverse(penetrated.Entity.transformation.Gyration)) - Math2d.Cross(penetrated.Entity.angularVelocity, ra);

                    float contactVel = Vector2.Dot(rv, Normal);
                    

                    float j = contactVel;

                    if (float.IsNaN(j))
                    {
                        throw new Exception("impulse scalar is NaN");
                    }

                    Vector2 impulse = Normal * 2.0F * (-j);
                    if(!penetrated.isStatic)
                    {
                        if (contactVel <= 0)
                            penetrated.ApplyImpulse(-impulse, ra);
                        penetrated.Entity.scene.DynamicsProvider.PositionalCorrection(penetrated.Entity.transformation, -Normal * penetration);
                    }
                    
                    if(!penetrating.isStatic)
                    {
                        if (contactVel <= 0)
                            penetrating.ApplyImpulse(impulse, rb);
                        penetrating.Entity.scene.DynamicsProvider.PositionalCorrection(penetrating.Entity.transformation, Normal * penetration);
                    }
                }
            }
        }
    }
}
