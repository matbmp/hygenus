using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class Transformation
    {
        public Vector2 Translation;
        public Quaternion Rotation;

        public Transformation()
        {
            Translation = Vector2.Zero;
            Rotation = Quaternion.Identity;
        }
        public Transformation(Vector2 translation, Quaternion rotation)
        {
            Translation = translation;
            Rotation = rotation;
        }
        public Transformation(Vector2 translation)
        {
            this.Translation = translation;
            this.Rotation = Quaternion.Identity;
        }
    }
}
