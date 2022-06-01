using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Engine;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui.Standard;

namespace Hygenus
{
    public class GuiScreens
    {
        public delegate void ShowScreen(GameTime gameTime);

        public ShowScreen CurrentScreen { get; set; }

        Hygenus game;
        private ImGUIRenderer GuiRenderer;
        private GraphicsDeviceManager graphics;

        private ImGuiWindowFlags utilityFlags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize;

        private System.Numerics.Vector2 mainMenuSize;
        private System.Numerics.Vector2 mainMenuPos;

        private System.Numerics.Vector2 levelEditorSize;
        private System.Numerics.Vector2 levelEditorPos;

        private string[] levels;
        

        public GuiScreens(Hygenus game, GraphicsDeviceManager graphics, ImGUIRenderer GuiRenderer)
        {
            this.game = game;
            this.GuiRenderer = GuiRenderer;
            this.graphics = graphics;
            CurrentScreen = MainMenu;
            levels = game.gameDataManager.getAllLevels();
            System.Diagnostics.Debug.WriteLine(levels.Length);
        }

        public void MainMenu(GameTime gameTime)
        {
            mainMenuSize = new System.Numerics.Vector2(300.0F, 300.0F);
            mainMenuPos = (new System.Numerics.Vector2(graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight) - mainMenuSize) / 2.0F;

            GuiRenderer.BeginLayout(gameTime);

            ImGui.SetNextWindowPos(mainMenuPos);
            ImGui.SetNextWindowSize(mainMenuSize);
            ImGui.Begin("Hygenus", utilityFlags);
            ImGui.SetWindowFontScale(4.0F);

            ImGui.SetCursorPosX((ImGui.GetWindowSize().X - ImGui.CalcTextSize("Start").X) * 0.5F);
            if (ImGui.Button("Start"))
            {
                CurrentScreen = Levels;
            }

            ImGui.SetCursorPosX((ImGui.GetWindowSize().X - ImGui.CalcTextSize("Wyjscie").X) * 0.5F);
            if (ImGui.Button("Wyjscie"))
            {
                game.Exit();
            }

            ImGui.End();

            GuiRenderer.EndLayout();
        }

        public void Levels(GameTime gameTime)
        {
            GuiRenderer.BeginLayout(gameTime);

            ImGui.SetNextWindowPos(mainMenuPos);
            ImGui.SetNextWindowSize(mainMenuSize);
            ImGui.Begin("Levels", utilityFlags);
            if (ImGui.BeginTable("levels", 1))
            {
                for (int row = 0; row < levels.Length; row++)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    
                    if (ImGui.Button(Path.GetFileName(levels[row])))
                    {
                        Level l = game.gameDataManager.getLevel(levels[row]);
                        game.scene.Entities.Clear();
                        foreach(Entity e in l.Entities)
                        {
                            
                            game.scene.addEntity(e);
                        }
                        game.player = l.Player;
                        CurrentScreen = Empty;
                        game.scene.Pause = false;
                    }
                }
                ImGui.EndTable();
            }

            ImGui.End();
            
            GuiRenderer.EndLayout();

        }


        private int selectedEntityIndex = 0;
        private int selectedComponentIndex = -1;
        private float rotation;
        private System.Numerics.Vector2 scale = new System.Numerics.Vector2(1.0F);
        private System.Numerics.Vector2 position;
        private float PK = 1.0F;
        public void LevelEditor(GameTime gameTime)
        {
            levelEditorPos = new System.Numerics.Vector2(5.0F, 5.0F);
            levelEditorSize = new System.Numerics.Vector2(400.0F, 800.0F);
            System.Numerics.Vector2 outerSize = new System.Numerics.Vector2(250.0F);
            GuiRenderer.BeginLayout(gameTime);

            ImGui.SetNextWindowPos(levelEditorPos);
            ImGui.SetNextWindowSize(levelEditorSize);
            ImGui.Begin("LevelEditor");

            ImGui.Text("Entities");
            
            if (ImGui.Button("Add new Entity"))
            {
                game.scene.addEntity(new Entity());
            }
            List<Entity> entities = game.scene.Entities;
            if (ImGui.BeginTable("Entities", 1,ImGuiTableFlags.Borders, outerSize, 250.0F))
            {
                ImGui.TableSetupColumn("Entities");
                
                for (int row = 0; row < entities.Count; row++)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    int color;
                    if (row == selectedEntityIndex)
                        color = System.Drawing.Color.Orange.ToArgb();
                    else
                        color = (row % 2 == 0) ? System.Drawing.Color.DarkGreen.ToArgb() : System.Drawing.Color.Green.ToArgb();
                    ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, (uint)color);
                    if(ImGui.Button(entities[row].Name, new System.Numerics.Vector2(250.0F, 20.0F)))
                    {
                        selectedEntityIndex = row;
                    }
                }
                ImGui.EndTable();
            }

            Entity selectedEntity = game.scene.Entities[selectedEntityIndex];

            if(ImGui.BeginTable("Entity Components", 1, ImGuiTableFlags.Borders, outerSize, 250.0F))
            {
                ImGui.TableSetupColumn("Entity Components");
                for(int row = 0; row < selectedEntity.Updatables.Count; row++)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    int color;
                    if (row == selectedComponentIndex)
                        color = System.Drawing.Color.Orange.ToArgb();
                    else
                        color = (row % 2 == 0) ? System.Drawing.Color.DarkGreen.ToArgb() : System.Drawing.Color.Green.ToArgb();
                    ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, (uint)color);
                    if (ImGui.Button(selectedEntity.Components[row].Name, new System.Numerics.Vector2(250.0F, 20.0F)))
                    {
                        if (selectedComponentIndex == row) selectedComponentIndex = -1;
                        else selectedComponentIndex = row;
                    }
                }
                ImGui.EndTable();
            }

            if(selectedComponentIndex >= 0 && selectedComponentIndex < selectedEntity.Components.Count)
            {
                Component selectedComponent = selectedEntity.Components[selectedComponentIndex];
                ImGui.SliderAngle("Rotation", ref rotation);
                selectedComponent.localTransformation.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Backward, rotation);
                ImGui.SliderFloat2("Position", ref position, -30, 30);
                Vector2 direction = new Vector2(position.X, position.Y);
                float len = direction.Length();
                direction.Normalize();
                direction *= 0.1F;
                Vector3 result = (new GyroVector(direction) * len).vec;
                selectedComponent.localTransformation.Translation = new Vector2(result.X, result.Y);
                ImGui.SliderFloat2("Scale", ref scale, -0.1F, 3F);
                selectedComponent.localTransformation.Scale = new Vector2(scale.X, scale.Y);
                if (ImGui.Button("Reset Component"))
                {
                    rotation = 0;
                    position = System.Numerics.Vector2.Zero;
                }
            }
            else
            {
                ImGui.SliderAngle("Rotation", ref rotation);
                selectedEntity.transformation.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Backward, rotation);
                ImGui.SliderFloat2("Position", ref position, -30, 30);
                Vector2 direction = new Vector2(position.X, position.Y);
                float len = direction.Length();
                direction.Normalize();
                direction *= 0.1F;
                Vector3 result = (new GyroVector(direction) * len).vec;
                selectedEntity.transformation.Translation = new Vector2(result.X, result.Y);

                

            }
            ImGui.SliderFloat("Poincare-Klein", ref PK, 0.15F, 2.0F);
            Renderer r = game.scene.Renderer;
            r.RenderEffect.Parameters["PK"].SetValue(PK);
            if (r.PostProcessEffect.Parameters["PK"] != null)
                r.PostProcessEffect.Parameters["PK"].SetValue(PK);

            if (ImGui.Button("Zapisz poziom"))
            {
                Level l = new Level("test", game.scene);
                l.Player = game.player;
                game.gameDataManager.saveLevel(l);
            }

            ImGui.End();

            GuiRenderer.EndLayout();
        }

        public void Empty(GameTime gameTime)
        {
            GuiRenderer.BeginLayout(gameTime);
            GuiRenderer.EndLayout();
        }
    }
}
