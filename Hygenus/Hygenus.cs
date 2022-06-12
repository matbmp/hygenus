using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui.Standard;
using Engine;
using System;

namespace Hygenus
{
    public class Hygenus : Game
    {
        private GraphicsDeviceManager graphics;
        public GameDataManager gameDataManager;
        public PoziomContext poziomContext;

        public Scene scene;
        public PlayerEntity player;
        public FinishLine finish;

        
        public Texture2D wallTexture;
        public Texture2D spaceshipTexture;
        public bool isLevelEditor = false;
        private GuiScreens guiScreens;
        public ImGUIRenderer GuiRenderer;

        public Hygenus()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            gameDataManager = new GameDataManager();
            poziomContext = new PoziomContext();
        }

        protected override void Initialize() {
            base.Initialize();

            //graphics.PreferMultiSampling = true;
            //graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 16;
            int h = Math.Min(graphics.GraphicsDevice.DisplayMode.Height, graphics.GraphicsDevice.DisplayMode.Width) - 100;
            graphics.PreferredBackBufferWidth = h;
            graphics.PreferredBackBufferHeight = h;
            if (isLevelEditor) graphics.PreferredBackBufferWidth += (1440 - h);
            graphics.ApplyChanges();
            
            HyperColorEffect HyperShader = new HyperColorEffect(Content.Load<Effect>(@"HyperShader"));
            Effect PostProcessShader = Content.Load<Effect>(@"PostProcess");

            float d = 1F;
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, -1), Vector3.Zero, Vector3.Up);
            
            Matrix projection = Matrix.CreateOrthographic(2 , 2, 0.1F, 100.0F);
            

            HyperShader.Projection = projection;
            HyperShader.View = view;
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
            scene.Renderer = new Renderer(GraphicsDevice, HyperShader, new Rectangle(isLevelEditor ? 400 : 0, 0, h, h));
            scene.Renderer.PostProcessEffect = PostProcessShader;
            scene.Renderer.RenderEffect.SpriteTexture = wallTexture;

            generateBackground();

            finish = new FinishLine();
            finish.transformation.Translation = new Vector2(0.0F, 0.4F);
            scene.addEntity(finish);
            player = new PlayerEntity(isLevelEditor);
            scene.addEntity(player);

            Dictionary<string, Texture2D> textures = scene.Renderer.RenderEffect.textures;
            textures.Add("futwall", Content.Load<Texture2D>("futwall"));
            textures.Add("spaceship2", Content.Load<Texture2D>("spaceship2"));


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

        private void generateBackground()
        {
            Entity background = new Entity("background");
            List<GyroVector> backgroundTilePositions = TileMap.createTiles(5, 6, out Polygon p);
            Color[] colors = new Color[] { new Color(26, 26, 189), new Color(204, 52, 14), new Color(220, 186, 227) };
            Color c;
            Color c2;
            double alpha;
            Random rand = new Random();
            foreach (GyroVector gv in backgroundTilePositions)
            {
                float cc = (float)rand.NextDouble();
                c = colors[0];
                for (int i = 1; i < colors.Length; i++)
                {
                    c2 = colors[i];

                    alpha = (i % 2 == 0) ? gv.vec.LengthSquared() : (gv.gyr.Z);
                    c.R = (byte)(c.R * alpha + (1 - alpha) * c2.R);
                    c.G = (byte)(c.G * alpha + (1 - alpha) * c2.G);
                    c.B = (byte)(c.B * alpha + (1 - alpha) * c2.B);
                }
                ColoredPolygonRenderer cpr = new ColoredPolygonRenderer(new Transformation(new Vector2(gv.vec.X, gv.vec.Y), (gv.gyr)), p, c);
                background.AddComponent(cpr);
            }
            scene.addEntity(background);
        }
        

    }
}
