using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGui.Standard;
using Engine;
using System;

namespace Hygenus
{
    public class Hygenus : Game
    {
        private GraphicsDeviceManager graphics;
        public GameDataManager gameDataManager;
        

        public Scene scene;
        public PlayerEntity player;

        private bool isLevelEditor = false;
        private GuiScreens guiScreens;
        public ImGUIRenderer GuiRenderer; //This is the ImGuiRenderer

        public Hygenus()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            gameDataManager = new GameDataManager();
        }

        protected override void Initialize() {
            base.Initialize();

            
            
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 960;
            if (isLevelEditor) graphics.PreferredBackBufferWidth += (1440 - 960);
            graphics.ApplyChanges();

            HyperColorEffect HyperShader = new HyperColorEffect(Content.Load<Effect>(@"HyperShader"));
            Effect PostProcessShader = Content.Load<Effect>(@"PostProcess");

            float d = 1F;
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, -d), Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreateOrthographic(2 , 2, 1.0F, 100.0F);

            HyperShader.ViewProjection = view * projection;
            HyperShader.K = HyperMath.K;
            HyperShader.Color = Color.Blue.ToVector4();
            if (PostProcessShader.Parameters["K"] != null)
                PostProcessShader.Parameters["K"].SetValue(HyperMath.K);
            if (PostProcessShader.Parameters["PK"] != null)
                PostProcessShader.Parameters["PK"].SetValue(1F);
            if (HyperShader.Parameters["PK"] != null)
                HyperShader.Parameters["PK"].SetValue(1F);

            scene = new Scene();
            scene.DynamicsProvider = new HyperbolicDynamicsProvider();
            scene.CollisionResolution = HyperbolicCollisionResolver.Resolve;
            scene.Transformer = new HyperbolicTransformer();
            scene.Renderer = new Renderer(GraphicsDevice, HyperShader, new Rectangle(isLevelEditor ? 400 : 0, 0, 960, 960));
            scene.Renderer.PostProcessEffect = PostProcessShader;


            Entity background = new Entity("background");
            List<PolygonRenderer> backgroundTiles = TileMap.createTiles(5, 6);
            foreach(PolygonRenderer r in backgroundTiles)
            {
                background.AddComponent(r);
            }
            scene.addEntity(background);

            FinishLine finish = new FinishLine();
            finish.transformation.Translation = new Vector2(0.0F, 0.4F);
            scene.addEntity(finish);
            player = new PlayerEntity();
            scene.addEntity(player);

            
            player.transformation.Gyration = Quaternion.CreateFromAxisAngle(Vector3.Forward, MathF.PI / 2);
            player.transformation.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Backward, MathF.PI / 2);

            GuiRenderer = new ImGUIRenderer(this).Initialize().RebuildFontAtlas();
            guiScreens = new GuiScreens(this, graphics, GuiRenderer);

            if (isLevelEditor)
            {
                guiScreens.CurrentScreen = guiScreens.LevelEditor;
            }
            else
            {
                guiScreens.CurrentScreen = guiScreens.MainMenu;
                scene.Pause = true;
            }

            

        }
        
        protected override void LoadContent()
        {
           
            
        }

        protected override void Update(GameTime gameTime)
        {
            scene.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            scene.Renderer.RenderEffect.CameraTranslation = new Vector3(-player.transformation.Translation, 0.0F);
            scene.Renderer.RenderEffect.CameraRotation = Quaternion.Inverse(player.transformation.Gyration);

            scene.Render();

            
            guiScreens.CurrentScreen(gameTime);

            base.Draw(gameTime);
        }
        

    }
}
