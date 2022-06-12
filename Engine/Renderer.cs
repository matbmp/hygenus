using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    /// <summary>
    /// Klasa wykonująca renderowanie komponentów z sceny
    /// </summary>
    public class Renderer
    {
        private SpriteBatch renderTargetSpriteBatch;
        private RenderTarget2D renderTarget;
        private GraphicsDevice graphics;
        private Rectangle output;

        // only one pass per effect supported
        public HyperColorEffect RenderEffect { get; set; }
        public Effect PostProcessEffect { get;  set; }

        public Renderer(GraphicsDevice graphics, HyperColorEffect renderEffect, Rectangle output, int superResolution = 3)
        {
            this.graphics = graphics;
            RenderEffect = renderEffect;
            renderTargetSpriteBatch = new SpriteBatch(graphics);
            this.output = output;
            setSuperResolution(superResolution);
        }
        

        public void Render(Scene scene)
        {
            if(PostProcessEffect != null)
            {
                graphics.SetRenderTarget(renderTarget);
            }
            graphics.BlendState = BlendState.AlphaBlend;
            graphics.DepthStencilState = new DepthStencilState() { DepthBufferEnable = false };
            RenderEffect.CurrentTechnique.Passes[0].Apply();
            for (int i = 0; i < scene.Entities.Count; i++)
            {
                var entity = scene.Entities[i];
                entity.Render(RenderEffect); // obecnie każdy element zmienia uniformy, do zaimplementowania batched rendering
            }
            if(PostProcessEffect != null)
            {
                graphics.BlendState = BlendState.Opaque;
                graphics.DepthStencilState = new DepthStencilState() { DepthBufferEnable = false };
                
                graphics.SetRenderTarget(null);
                renderTargetSpriteBatch.Begin(effect: PostProcessEffect);
                renderTargetSpriteBatch.Draw(renderTarget, output, Color.White);
                renderTargetSpriteBatch.End();
            }
        }

        /// <summary>
        /// Ustawienie większej pośredniej rozdzielczości renderowania niż docelowa
        /// </summary>
        /// <param name="superResolution">mnożnik rozdielczości</param>
        /// <exception cref="ArgumentException"></exception>
        public void setSuperResolution(int superResolution)
        {
            if (superResolution <= 0) throw new ArgumentException("mnożnik rozdielczości musi być większy od 0");
            renderTarget = new RenderTarget2D(graphicsDevice: graphics,
                                                width: output.Width * superResolution,
                                                output.Height * superResolution,
                                                mipMap: false,
                                                preferredFormat: SurfaceFormat.Color,
                                                preferredDepthFormat: DepthFormat.Depth24);
        }
    }
}
