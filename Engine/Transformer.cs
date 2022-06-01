using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public interface Transformer
    {
        public Transformation Combine(Transformation first, Transformation second);
    }
}
