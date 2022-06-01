using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class EuclideanTransformer : Transformer
    {
        public Transformation Combine(Transformation first, Transformation second)
        {
            Transformation result = new Transformation();
            result.Translation = first.Translation + Vector2.Transform(second.Translation, first.Rotation);
            result.Rotation = first.Rotation * second.Rotation;
            return result;
        }
    }
}
