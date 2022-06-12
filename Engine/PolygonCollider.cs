using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    [DataContract]
    public class PolygonCollider : Component, IUpdatable
    {
        public delegate void OnCollisionWith(PolygonCollider other);
        [DataMember]
        public Polygon polygon;
        [DataMember]
        private Transformation worldTransformation = new Transformation();
        public Transformation WorldTransformation { get => worldTransformation; }
        [DataMember]
        public bool isStatic { get; set; }
        [DataMember]
        public bool collisionResolution = true;

        public OnCollisionWith OnCollided;

        public Vector2[] WorldPoints;
        protected Vector2[] WorldEdgeNormals;


        public PolygonCollider()
        {

        }
        public PolygonCollider(Vector2[] points) : base()
        {
            WorldPoints = new Vector2[points.Length];
            WorldEdgeNormals = new Vector2[points.Length];
            localTransformation = new Transformation();
            localTransformation.Translation = Polygon.RecenterPoints(points);
            polygon = new Polygon(points);
            isStatic = false;
        }

        public virtual void Update()
        {
            UpdateWorldTransformation();
            Vector2[] originalPoints = polygon.Points;
            for(int i = 0; i < originalPoints.Length; i++)
            {
                WorldPoints[i] = localTransformation.Translation + Vector2.Transform(originalPoints[i], localTransformation.Rotation);
            }
        }
        protected void UpdateWorldTransformation()
        {
            Transformation t = Entity.scene.Transformer.Combine(Entity.transformation, localTransformation);
            WorldTransformation.Translation = t.Translation;
            WorldTransformation.Rotation = t.Rotation;
            WorldTransformation.Gyration = t.Gyration;
        }

        public virtual void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
        {
            Entity.ApplyImpulse(impulse, contactVector);
        }

        public virtual void OnCollidedWith(PolygonCollider other)
        {
            if(OnCollided != null)
            OnCollided(other);
        }

        public static void CheckCollision(PolygonCollider a, PolygonCollider b, out CollisionResult result)
        {
            result = null;
            CalculatePenetration(a, b, out int aEdge, out float aPenetration);
            if (aPenetration > 0) return;
            CalculatePenetration(b, a, out int bEdge, out float bPenetration);
            if (bPenetration > 0) return;

            bool flip = false;
            if (aPenetration > bPenetration)
            {
                // a jest ścianą
            }
            else
            {
                // b jest ścianą
                PolygonCollider temp = a;
                a = b; b = temp;
                aEdge = bEdge;
                flip = true;
            }
            // a jest ścianą
            Vector2 EdgeNormal = a.WorldEdgeNormals[aEdge];
            float leastProjection = float.PositiveInfinity;
            int penetratingEdgeIndex = 0;
            for (int i = 0; i < b.WorldPoints.Length; i++)
            {
                float projection = Vector2.Dot(EdgeNormal, b.WorldEdgeNormals[i]);
                if (projection < leastProjection)
                {
                    leastProjection = projection;
                    penetratingEdgeIndex = i;
                }
            }
            Vector2[] penetratingEdge = new Vector2[2];
            penetratingEdge[0] = b.WorldPoints[penetratingEdgeIndex];
            penetratingEdge[1] = b.WorldPoints[(penetratingEdgeIndex + 1) % b.WorldPoints.Length];


            Vector2 v1, v2;
            v1 = a.WorldPoints[aEdge];
            v2 = a.WorldPoints[(aEdge + 1) % a.WorldPoints.Length];
            Vector2 sidePlaneNormal = (v2 - v1);
            sidePlaneNormal.Normalize();
            Vector2 refFaceNormal = new Vector2(sidePlaneNormal.Y, -sidePlaneNormal.X);


            float refC = Vector2.Dot(refFaceNormal, v1);
            float negSide = -Vector2.Dot(sidePlaneNormal, v1);
            float posSide = Vector2.Dot(sidePlaneNormal, v2);


            if (Clip(-sidePlaneNormal, negSide, penetratingEdge) < 2)
                return;

            if (Clip(sidePlaneNormal, posSide, penetratingEdge) < 2)
                return;

            float penetration;
            Vector2[] contacts = new Vector2[2];
            int cp = 0;
            float separation = Vector2.Dot(refFaceNormal, penetratingEdge[0]) - refC;
            if (separation <= 0.0f)
            {
                contacts[cp++] = penetratingEdge[0];
                penetration = -separation;
            }
            else
                penetration = 0;

            separation = Vector2.Dot(refFaceNormal, penetratingEdge[1]) - refC;
            if (separation <= 0.0f)
            {
                contacts[cp++] = penetratingEdge[1];

                penetration += -separation;

                penetration /= cp;
            }
            if (penetration > 0 && (contacts[0] != Vector2.Zero || contacts[1] != Vector2.Zero))
            result = new CollisionResult(contacts, refFaceNormal, penetration, a, b);

        }

        private int SupportPointIndex(Vector2 direction)
        {
            int best = 0;
            float bestProjection = Vector2.Dot(WorldPoints[best], direction);
            for(int i = 1; i < WorldPoints.Length; i++)
            {
                float projection = Vector2.Dot(WorldPoints[i], direction);
                if (projection < bestProjection)
                {
                    bestProjection = projection;
                    best = i;
                }
            }
            return best;
        }
        private static void CalculatePenetration(PolygonCollider a, PolygonCollider b, out int edgeLeastPenetrated, out float leastPenetration)
        {
            leastPenetration = float.NegativeInfinity;
            edgeLeastPenetrated = 0;
            for(int i = 0; i < a.WorldEdgeNormals.Length; i++)
            {
                Vector2 axis = a.WorldEdgeNormals[i];
                int supportB = b.SupportPointIndex(axis);
                float penetration = Vector2.Dot(b.WorldPoints[supportB] - a.WorldPoints[i], axis);
                
                if(penetration > leastPenetration)
                {
                    leastPenetration = penetration;
                    edgeLeastPenetrated = i;
                    if (penetration > 0) return;
                }
            }
        }

        private static int Clip(Vector2 n, float c, Vector2[] face)
        {
            int sp = 0;
            Vector2[] result = {
                face[0],
                face[1]
            };

            // Retrieve distances from each endpoint to the line
            // d = ax + by - c
            float d1 = Vector2.Dot(n, face[0]) - c;
            float d2 = Vector2.Dot(n, face[1]) - c;

            // If negative (behind plane) clip
            if (d1 <= 0.0f) result[sp++] = face[0];
            if (d2 <= 0.0f) result[sp++] = face[1];

            // If the points are on different sides of the plane
            if (d1 * d2 < 0.0f) // less than to ignore -0.0f
            {
                // Push interesection point
                float alpha = d1 / (d1 - d2);
                result[sp] = face[0] + alpha * (face[1] - face[0]);
                ++sp;
            }

            // Assign our new converted values
            face[0] = result[0];
            face[1] = result[1];

            if (sp == 3)
            {
                throw new Exception("sp = 3");
            }
            return sp;
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext c)
        {
            WorldPoints = new Vector2[polygon.Points.Length];
            WorldEdgeNormals = new Vector2[polygon.Points.Length];
        }
    }
}
