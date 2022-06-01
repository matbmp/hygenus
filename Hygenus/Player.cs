using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;
using Engine;
using Microsoft.Xna.Framework.Input;

namespace Hygenus
{
    public class PlayerEntity : Entity
    {
        private Controls controls;
        private float turnRight;
        private float forceForward;

        public PlayerEntity() : base("Player")
        {
            controls = new Controls() { Stop = Keys.S, Left = Keys.A, Right = Keys.D };

            float size = 0.05F;
            Vector2 a, b, c, d;
            a = Vector2.Transform(new Vector2(size, 0), Quaternion.CreateFromAxisAngle(Vector3.Backward, MathF.PI / 2));
            b = Vector2.Transform(new Vector2(size, 0), Quaternion.CreateFromAxisAngle(Vector3.Backward, -MathF.PI / 6));
            c = Vector2.Transform(new Vector2(size, 0), Quaternion.CreateFromAxisAngle(Vector3.Backward, MathF.PI * 7 / 6));
            d = Vector2.Zero;
            HyperPolygonCollider triangle1 = new HyperPolygonCollider(c, d, a);
            HyperPolygonCollider triangle2 = new HyperPolygonCollider(d, b, a);
            this.AddComponent(triangle1);
            this.AddComponent(triangle2);
            this.AddComponent(new PolygonRenderer(triangle1));
            this.AddComponent(new PolygonRenderer(triangle2));
        }

        public override void Update()
        {
            base.Update();
            HandleControls();
            transformation.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.Forward, turnRight);
            this.ApplyImpulse(Vector2.Transform(Vector2.UnitY * forceForward, Quaternion.Inverse(this.transformation.Rotation)), Vector2.Zero);
            this.ApplyImpulse(-velocity / 80, Vector2.Zero);
            this.angularVelocity *= 0.98F;
        }
        private void HandleControls()
        {
            KeyboardState state = Keyboard.GetState();
            float moveSpeed = 0.00005F;
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
            if (state.IsKeyDown(controls.Stop))
            {
                forceForward = 0.0F;
            }
            else
            {
                forceForward = moveSpeed;
            }
        }
    }
}
