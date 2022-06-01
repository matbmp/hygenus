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
            for (int i = 0; i < result.contacts.Length; i++)
            {
                if (contacts[i] != null)
                {
                    Vector2 ra = contacts[i] - GyroVector.PoincareToKlein(penetrated.Entity.transformation.Translation);
                    Vector2 rb = contacts[i] - GyroVector.PoincareToKlein(penetrating.Entity.transformation.Translation);
                    

                    // relative velocity
                    Vector2 rv = Vector2.Transform(penetrating.Entity.velocity, Quaternion.Inverse(penetrating.Entity.transformation.Gyration)) + Math2d.Cross(penetrating.Entity.angularVelocity, rb) -
                        Vector2.Transform(penetrated.Entity.velocity, Quaternion.Inverse(penetrated.Entity.transformation.Gyration)) - Math2d.Cross(penetrated.Entity.angularVelocity, ra);

                    float contactVel = Vector2.Dot(rv, Normal);
                    

                    float j = contactVel;

                    if (float.IsNaN(j))
                    {
                        throw new Exception("impulse scalar is NaN");
                    }

                    Vector2 impulse = Normal * j;
                    impulse *= -1.4F;
                    impulse /= 1F;
                    if(impulse.LengthSquared() > 0.001F)
                    {

                    }
                    if(!penetrated.isStatic)
                    {
                        if (contactVel <= 0)
                            penetrated.ApplyImpulse(Vector2.Transform(-impulse, Quaternion.Identity), ra);
                        penetrated.Entity.scene.DynamicsProvider.PositionalCorrection(penetrated.Entity.transformation, -Normal * penetration);
                    }
                    
                    if(!penetrating.isStatic)
                    {
                        if (contactVel <= 0)
                            penetrating.ApplyImpulse(Vector2.Transform(impulse, Quaternion.Identity), rb);
                        penetrating.Entity.scene.DynamicsProvider.PositionalCorrection(penetrating.Entity.transformation, Normal * penetration);
                    }
                }
            }
        }
    }
}
