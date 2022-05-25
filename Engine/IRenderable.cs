using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public interface IRenderable
    {
        public void Render(HyperColorEffect effect);
    }
}
