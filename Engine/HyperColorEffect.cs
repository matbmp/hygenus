using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    /// <summary>
    /// Shader hiperboliczny
    /// TODO : przenieść z Engine do Hygenus
    /// </summary>
    public class HyperColorEffect : Effect
    {
        public Matrix ViewProjection { set { this.Parameters["viewProjection"].SetValue(value); } }
        public Vector4 Color { set { this.Parameters["color"].SetValue(value); } }
        public Vector3 CameraTranslation { set { this.Parameters["cameraTranslation"].SetValue(value); } }
        public Quaternion CameraRotation { set { this.Parameters["cameraRotation"].SetValue(value); } }
        public Vector3 ObjectTranslation { set { this.Parameters["objectTranslation"].SetValue(value); } }
        public Quaternion ObjectRotation { set { this.Parameters["objectRotation"].SetValue(value); } }
        public Vector3 ObjectScale { set { this.Parameters["objectScale"].SetValue(value); } }
        public float K { set { this.Parameters["K"].SetValue(value); } }

        public HyperColorEffect(Effect cloneSource) : base(cloneSource)
        {
            
        }


    }
}
