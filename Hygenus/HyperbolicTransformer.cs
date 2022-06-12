using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;

namespace Hygenus
{
    public class HyperbolicTransformer : Transformer
    {
        public Transformation Combine(Transformation first, Transformation second)
        {
            Transformation result = new Transformation();
            GyroVector trans = new GyroVector(second.Translation);
            GyroVector gv = new GyroVector(first.Translation, first.Gyration * first.Rotation) + trans;
            result.Translation = new Vector2(gv.vec.X, gv.vec.Y);
            result.Rotation = gv.gyr * second.Rotation;
            result.Gyration = second.Gyration;
            return result;
        }
    }
}
