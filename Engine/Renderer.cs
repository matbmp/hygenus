﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class Renderer
    {
        private SpriteBatch renderTargetSpriteBatch;
        private RenderTarget2D renderTarget;
        private GraphicsDevice graphics;

        // only one pass per effect supported
        private HyperColorEffect RenderEffect;
        public Effect PostProcessEffect { private get;  set; }

        public Renderer(GraphicsDevice graphics, HyperColorEffect renderEffect, int superResolution = 3)
        {
            this.graphics = graphics;
            RenderEffect = renderEffect;
            renderTargetSpriteBatch = new SpriteBatch(graphics);
            setSuperResolution(3);
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

        public void Render(Scene scene)
        {
            if(PostProcessEffect != null)
            {
                graphics.SetRenderTarget(renderTarget);
            }
            graphics.BlendState = BlendState.Opaque;
            graphics.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            graphics.SamplerStates[0] = SamplerState.LinearWrap;
            RenderEffect.CurrentTechnique.Passes[0].Apply();
            for (int i = 0; i < scene.Entities.Count; i++)
            {
                var entity = scene.Entities[i];
                entity.Render(RenderEffect);
            }
            if(PostProcessEffect != null)
            {
                PostProcessEffect.CurrentTechnique.Passes[0].Apply();
                graphics.SetRenderTarget(null);
                renderTargetSpriteBatch.Begin(effect: PostProcessEffect);
                renderTargetSpriteBatch.Draw(renderTarget, graphics.PresentationParameters.Bounds, Color.White);
                renderTargetSpriteBatch.End();
            }
        }
    }
}