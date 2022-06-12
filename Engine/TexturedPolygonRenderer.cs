using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    [DataContract]
    public class TexturedPolygonRenderer : Component, IRenderable
    {
        [DataMember]
        private VertexPositionTexture[] renderVertices;
        [DataMember]
        private short[] renderIndices;
        [DataMember]
        private string textureName;
        

        public TexturedPolygonRenderer(PolygonCollider polygonCollider, Vector2[] textureUVs, short[] renderIndices, string textureName)
        {
            localTransformation = polygonCollider.localTransformation;
            Polygon polygon = polygonCollider.polygon;
            if (polygon.Points.Length != textureUVs.Length) throw new ArgumentException("Number of vertices does not equal the number of texture UVs");
            this.textureName = textureName;
            this.renderIndices = renderIndices;
            renderVertices = new VertexPositionTexture[polygon.Points.Length];
            for (int i = 0; i < polygon.Points.Length; i++)
            {
                renderVertices[i] = new VertexPositionTexture(new Vector3(polygon.Points[i], 0.0F), textureUVs[i]);
            }
        }

        public TexturedPolygonRenderer(Transformation transformation, Vector2[] Points,  Vector2[] textureUVs, short[] renderIndices, string textureName)
        {
            localTransformation = transformation;
            this.textureName = textureName;
            this.renderIndices = renderIndices;
            renderVertices = new VertexPositionTexture[Points.Length];
            for (int i = 0; i < Points.Length; i++)
            {
                renderVertices[i] = new VertexPositionTexture(new Vector3(Points[i], 0.0F), textureUVs[i]);
            }
        }

        public void Render(HyperColorEffect effect)
        {
            Transformer transformer = Entity.scene.Transformer;
            Transformation final = transformer.Combine(Entity.transformation, localTransformation);
            effect.Color = Color.White.ToVector4();
            effect.ObjectTranslation = new Vector3(final.Translation, 0.0F);
            effect.ObjectRotation = final.Rotation;
            effect.ObjectScale = new Vector3(localTransformation.Scale, 0F);
            effect.SpriteTexture = effect.textures.GetValueOrDefault(textureName);
            effect.IgnoreTexture = false;
            effect.CurrentTechnique.Passes[0].Apply();

            effect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, renderVertices, 0, renderVertices.Length, renderIndices, 0, renderIndices.Length/3);

        }
    }
}
