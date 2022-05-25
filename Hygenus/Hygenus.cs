using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGui.Standard;
using Engine;

namespace Hygenus
{
    public class Hygenus : Game
    {
        private GraphicsDeviceManager graphics;
        public ShaderParameters shaderParameters;
        

        private Scene scene;

        private GuiScreens guiScreens;
        public ImGUIRenderer GuiRenderer; //This is the ImGuiRenderer

        public Hygenus()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
        }

        protected override void Initialize() {
            base.Initialize();

            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.ApplyChanges();

            HyperColorEffect HyperShader = new HyperColorEffect(Content.Load<Effect>(@"HyperShader"));
            Effect PostProcessShader = Content.Load<Effect>(@"PostProcess");
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, -3), Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreateOrthographic(2, 2, 1.0F, 100.0F);

            HyperShader.ViewProjection = view * projection;
            HyperShader.K = HyperMath.K;
            HyperShader.Color = Color.Blue.ToVector4();
            if (PostProcessShader.Parameters["K"] != null)
                PostProcessShader.Parameters["K"].SetValue(HyperMath.K);

            scene = new Scene();
            scene.Renderer = new Renderer(GraphicsDevice, HyperShader);
            scene.Renderer.PostProcessEffect = PostProcessShader;


            Entity background = new Entity("background");
            List<PolygonRenderer> backgroundTiles = TileMap.createTiles(5, 6);
            foreach(PolygonRenderer r in backgroundTiles)
            {
                background.AddComponent(r);
            }
            scene.addEntity(background);


            Entity e = new Entity();
            PolygonCollider pc = new HyperPolygonCollider(Figures.Quad(0.2F, 0.2F));
            pc.velocity = new Vector2(0.003F, 0.0F);
            pc.angularVelocity = 0.01F;
            e.AddComponent(pc);
            e.AddComponent(new PolygonRenderer(pc));
            //e.AddComponent(new PolygonColliderRenderer(pc));
            scene.addEntity(e);

            Entity e2 = new Entity();
            pc = new HyperPolygonCollider(Figures.Quad(0.2F, 0.7F));
            pc.transformation.Translation = new Vector2(0.5F, 0.0F);
            e2.AddComponent(pc);
            e2.AddComponent(new PolygonRenderer(pc));
            //e2.AddComponent(new PolygonColliderRenderer(pc));
            scene.addEntity(e2);




            GuiRenderer = new ImGUIRenderer(this).Initialize().RebuildFontAtlas();
            guiScreens = new GuiScreens(this, graphics, GuiRenderer);

        }
        
        protected override void LoadContent()
        {
            //background = new List<Tile>();//TileMap.createTiles(5, 7, renderer);
           
            
        }

        protected override void Update(GameTime gameTime)
        {

            // TODO: Add your update logic here


            scene.Update();


            //scene.Update();
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            scene.Render();

            //GuiRenderer.BeginLayout(gameTime);
            //GuiRenderer.EndLayout();
            guiScreens.CurrentScreen(gameTime);

            base.Draw(gameTime);
        }
        

    }
}
