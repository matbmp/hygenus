using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Hygenus
{
    public class ShaderParameters
    {
        public EffectParameter viewProjection;
        public EffectParameter transformation;
        public EffectParameter gyration;
        private EffectParameter hyperK;
        private EffectParameter postK;

        public ShaderParameters(Effect hyper, Effect postProcess)
        {
            viewProjection = hyper.Parameters["viewProjection"];
            transformation = hyper.Parameters["transformation"];
            gyration = hyper.Parameters["gyration"];
            hyperK = hyper.Parameters["K"];
            postK = postProcess.Parameters["K"];
        }

        public void KSetValue(float k)
        {
            hyperK.SetValue(k);
            if(postK != null)
                postK.SetValue(k);
        }
    }
}
