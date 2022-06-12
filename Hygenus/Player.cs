using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static Engine.PolygonCollider;

namespace Hygenus
{
    public class PlayerEntity : Entity
    {
        private Controls controls = new Controls() { Forward = Keys.W, Stop = Keys.S, Left = Keys.A, Right = Keys.D };
        private int lastTickBadCollsion = -100;
        private float turnRight;
        private float forceForward;
        private float boost;
        private bool isLevelEditor;

        public PlayerEntity()
        {

        }

        public PlayerEntity(bool isLevelEditor) : base("Player")
        {
            this.isLevelEditor = isLevelEditor;

            float size = 0.04F;
            Vector2 a, b, c, d;
            a = Vector2.Transform(new Vector2(size, 0), Quaternion.CreateFromAxisAngle(Vector3.Backward, MathF.PI / 2));
            b = Vector2.Transform(new Vector2(size, 0), Quaternion.CreateFromAxisAngle(Vector3.Backward, -MathF.PI * 2 / 6));
            c = Vector2.Transform(new Vector2(size, 0), Quaternion.CreateFromAxisAngle(Vector3.Backward, MathF.PI * 8 / 6));
            d = new Vector2(0F, -size/2);
            HyperPolygonCollider body1 = new HyperPolygonCollider(c, d, a);
            HyperPolygonCollider body2 = new HyperPolygonCollider(d, b, a);
            HyperPolygonCollider booster = new HyperPolygonCollider(b, c, c + new Vector2(0F, -size*2), b + new Vector2(0F, -size*2) );
            booster.collisionResolution = false;
            booster.OnCollided +=  delegate (PolygonCollider other)
            {
                boost = 0.00010F;
            };
            OnCollisionWith BadCollsion = delegate (PolygonCollider other)
            {
                if(!(other.Entity is FinishLine))
                {
                    lastTickBadCollsion = tick;
                    this.velocity *= 0.65F;
                }
                
            };
            body1.OnCollided += BadCollsion;
            body2.OnCollided += BadCollsion;
            this.AddComponent(body1);
            this.AddComponent(body2);
            this.AddComponent(booster);
            float texScale = 0.5F;
            Vector2 disp = new Vector2(0.5F);
            TexturedPolygonRenderer tpr1 = new TexturedPolygonRenderer(body1.localTransformation, body1.polygon.Points,
                new Vector2[] { body1.polygon.Points[0] * texScale + disp, body1.polygon.Points[1] * texScale + disp, body1.polygon.Points[2] * texScale + disp },
                new short[] { 0, 1, 2},
                "spaceship2");
            TexturedPolygonRenderer tpr2 = new TexturedPolygonRenderer(body2.localTransformation, body2.polygon.Points,
                new Vector2[] { body2.polygon.Points[0] * texScale + disp, body2.polygon.Points[1] * texScale + disp, body2.polygon.Points[2] * texScale + disp },
                new short[] { 0, 1, 2 },
                "spaceship2");
            ColoredPolygonRenderer boosterRender = new ColoredPolygonRenderer(booster.localTransformation, booster.polygon, new Color(255, 0, 0, 64));
            this.AddComponent(tpr1);
            this.AddComponent(tpr2);
            this.AddComponent(boosterRender);
        }

        public override void Update()
        {
            base.Update();
            tick++;
            HandleControls();
            transformation.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Forward, turnRight);
            if(!isLevelEditor)
            {
                this.ApplyImpulse(Vector2.Transform(Vector2.UnitY * forceForward, Quaternion.Inverse(this.transformation.Rotation)), Vector2.Zero);
            }
            else
            {
                this.scene.DynamicsProvider.ApplyVelocity(this.transformation, Vector2.Transform(Vector2.UnitY * forceForward * 60, Quaternion.Inverse(this.transformation.Rotation)));
            }
            
            this.ApplyImpulse(-velocity / 80, Vector2.Zero);
            this.angularVelocity *= 0.98F;
        }
        private void HandleControls()
        {
            KeyboardState state = Keyboard.GetState();
            boost *= 0.98F;
            float moveSpeed;
            if (tick - lastTickBadCollsion > 20)
            {
                moveSpeed = 0.00005F + boost;
            }
            else
            {
                moveSpeed = 0.00003F + boost/2.0F;
            }
            
            float turn = 0.025F;
            if (state.IsKeyDown(controls.Left))
            {
                turnRight = -turn;
            }
            else if (state.IsKeyDown(controls.Right))
            {
                turnRight = turn;
            }
            else
            {
                turnRight = 0.0F;
            }
            if (!isLevelEditor)
            {
                if (state.IsKeyDown(controls.Stop))
                {
                    forceForward = 0.0F;
                }
                else
                {
                    forceForward = moveSpeed;
                }
            }
            else
            {
                if (state.IsKeyDown(controls.Forward))
                {
                    forceForward = moveSpeed;
                }
                else if (state.IsKeyDown(controls.Stop))
                {
                    forceForward = -moveSpeed;
                }
                else
                {
                    forceForward = 0;
                }
            }
            
        }
    }
}
