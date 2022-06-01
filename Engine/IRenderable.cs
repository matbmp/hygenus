using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    /// <summary>
    /// Interfejs implementowany przez wszystkie klasy dokonujące rysowania na ekranie
    /// </summary>
    public interface IRenderable
    {
        public void Render(HyperColorEffect effect);
    }
}
