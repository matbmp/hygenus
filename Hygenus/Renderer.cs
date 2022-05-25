using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hygenus2D
{
    public class Renderer
    {
        private SpriteBatch spriteBatch;
        private RenderTarget2D renderTarget;
        private GraphicsDevice graphics;
        private Effect standardShader;
        private Effect postProcessShader;
        List<VertexPositionColor[]> renderables;

        public Renderer(GraphicsDevice graphics, Effect standardShader, Effect postProcessShader, int superResolution = 3)
        {
            this.graphics = graphics;
            this.standardShader = standardShader;
            this.postProcessShader = postProcessShader;
            this.renderables = new List<VertexPositionColor[]>();
            spriteBatch = new SpriteBatch(graphics);
            setSuperResolution(superResolution);
        }
        public void setSuperResolution(int superResolution)
        {
            renderTarget = new RenderTarget2D(graphics,
                                               graphics.PresentationParameters.BackBufferWidth * superResolution,
                                               graphics.PresentationParameters.BackBufferHeight * superResolution,
                                               false,
                                               graphics.PresentationParameters.BackBufferFormat,
                                               DepthFormat.Depth24);
        }

        public void Consume(VertexPositionColor[] v)
        {
            renderables.Add(v);
        }

        public void performRender()
        {
            graphics.BlendState = BlendState.Opaque;
            graphics.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            graphics.SamplerStates[0] = SamplerState.LinearWrap;
            standardShader.CurrentTechnique.Passes[0].Apply();
            graphics.SetRenderTarget(renderTarget);
            foreach(VertexPositionColor[] v in renderables)
            {
                graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, v, 0, v.Length - 2);
            }

            graphics.SetRenderTarget(null);
            spriteBatch.Begin(effect: postProcessShader);
            spriteBatch.Draw(renderTarget, graphics.PresentationParameters.Bounds, Color.White);
            spriteBatch.End();

            renderables.Clear();
        }
    }
}
