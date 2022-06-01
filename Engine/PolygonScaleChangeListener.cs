using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public interface PolygonScaleChangeListener
    {
        public void updateScale(Polygon p, Vector2 scale);
    }
}
