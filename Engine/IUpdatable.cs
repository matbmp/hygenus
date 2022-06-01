using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    /// <summary>
    /// Interfejs implementowany przez wszsykie klasy wymagające aktualizacji w czasie gry
    /// </summary>
    public interface IUpdatable
    {
        public void Update()
        {
        }
    }
}
