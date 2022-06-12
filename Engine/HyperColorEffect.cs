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
        public Matrix View { set { this.Parameters["view"].SetValue(value); } }
        public Matrix Projection { set { if (this.Parameters["projection"] != null) this.Parameters["projection"].SetValue(value); } }
        public Vector4 Color { set { this.Parameters["color"].SetValue(value); } }
        public Vector3 CameraTranslation { set { if(this.Parameters["cameraTranslation"] != null) this.Parameters["cameraTranslation"].SetValue(value); } }
        public Quaternion CameraRotation { set { if (this.Parameters["cameraRotation"] != null) this.Parameters["cameraRotation"].SetValue(value); } }
        public Vector3 ObjectTranslation { set { this.Parameters["objectTranslation"].SetValue(value); } }
        public Quaternion ObjectRotation { set { this.Parameters["objectRotation"].SetValue(value); } }
        public Vector3 ObjectScale { set { this.Parameters["objectScale"].SetValue(value); } }
        public float K { set { this.Parameters["K"].SetValue(value); } }
        public float Par { set { if (this.Parameters["par"] != null) this.Parameters["par"].SetValue(value); } }
        public Texture2D SpriteTexture { set { this.Parameters["SpriteTexture"].SetValue(value); } }
        public bool IgnoreTexture { set { this.Parameters["ignore_texture"].SetValue(value); } }

        public Dictionary<string, Texture2D> textures { get; } = new Dictionary<string, Texture2D>();

        public HyperColorEffect(Effect cloneSource) : base(cloneSource)
        {
            
        }


    }
}
