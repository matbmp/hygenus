using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    /// <summary>
    /// Interfejs określający w jaki sposób "dodawać" do siebie transformacje, wykorzystywany przez Scene
    /// </summary>
    public interface Transformer
    {
        public Transformation Combine(Transformation first, Transformation second);
    }
}
