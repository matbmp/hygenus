using System;
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

        private List<string> levels;
        

        public GuiScreens(Hygenus game, GraphicsDeviceManager graphics, ImGUIRenderer GuiRenderer)
        {
            this.game = game;
            this.GuiRenderer = GuiRenderer;
            this.graphics = graphics;
            CurrentScreen = MainMenu;
            var poziomy = game.poziomContext.Poziomy;
            
            levels = new List<string>();
            foreach(Poziom p in poziomy)
            {
                levels.Add(p.nazwa);
            }
            
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
                for (int row = 0; row < levels.Count; row++)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    
                    if (ImGui.Button(Path.GetFileName(levels[row])))
                    {
                        Level l = game.gameDataManager.getLevel(levels[row]);
                        game.scene.Entities.Clear();
                        foreach(Entity e in l.Entities)
                        {
                            if(!(e is FinishLine))
                            {
                                if (!(e is PlayerEntity))
                                    game.scene.addEntity(e);
                            }
                            else
                            {
                                FinishLine f = new FinishLine();
                                f.transformation = e.transformation;
                                for(int i = 0; i < e.Components.Count; i++)
                                {
                                    f.Components[i].localTransformation = e.Components[i].localTransformation;
                                }
                                game.scene.addEntity(f);
                            }
                            
                        }
                        game.player.transformation = l.Player.transformation;
                        game.scene.addEntity(game.player);
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
        private Vector2 oldPosition;
        bool touch;
        private float PK = 1.0F;
        private float freepar;
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
                        selectedComponentIndex = -1;

                        
                        Entity selectedEntity2 = game.scene.Entities[selectedEntityIndex];
                        if (!(selectedComponentIndex >= 0 && selectedComponentIndex < selectedEntity2.Components.Count))
                        {
                            Quaternion q = selectedEntity2.transformation.Rotation;
                            
                            rotation = 2 * MathF.Atan2(q.Z, q.W);
                            scale = new System.Numerics.Vector2(selectedEntity2.transformation.Scale.X, selectedEntity2.transformation.Scale.Y);
                            position = new System.Numerics.Vector2(selectedEntity2.transformation.Translation.X, selectedEntity2.transformation.Translation.Y);
                        }
                            
                    }
                }
                ImGui.EndTable();
            }

            Entity selectedEntity = game.scene.Entities[selectedEntityIndex];

            if(ImGui.Button("Add wall to entity"))
            {
                var hpc = new HyperPolygonCollider(Figures.Quad(0.5F, 0.5F));
                hpc.isStatic = true;
                var tpr = new TexturedPolygonRenderer(hpc.localTransformation, hpc.polygon.Points,
                    new Vector2[] { new Vector2(0F, 0F), new Vector2(0F, 1F), new Vector2(1F, 1F), new Vector2(1F, 0F) },
                    new short[] { 0, 1, 2, 0, 2, 3 },
                    "futwall");
                selectedEntity.AddComponent(hpc);
                selectedEntity.AddComponent(tpr);
            }

            if(ImGui.BeginTable("Entity Components", 1, ImGuiTableFlags.Borders, outerSize, 250.0F))
            {
                ImGui.TableSetupColumn("Entity Components");
                for(int row = 0; row < selectedEntity.Components.Count; row++)
                {
                    if (!(selectedEntity.Components[row] is IUpdatable)) continue;
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

                        if(selectedComponentIndex >= 0 && selectedComponentIndex < selectedEntity.Components.Count)
                        {
                            Component selectedComponent = selectedEntity.Components[selectedComponentIndex];
                            Quaternion q = selectedComponent.localTransformation.Rotation;
                            rotation = 2 * MathF.Atan2(q.Z, q.W);
                            scale = new System.Numerics.Vector2(selectedComponent.localTransformation.Scale.X, selectedComponent.localTransformation.Scale.Y);
                            position = new System.Numerics.Vector2(selectedComponent.localTransformation.Translation.X, selectedComponent.localTransformation.Translation.Y);
                        }
                        else
                        {
                            Quaternion q = selectedEntity.transformation.Rotation;
                            rotation = 2 * MathF.Atan2(q.Z, q.W);
                            scale = new System.Numerics.Vector2(selectedEntity.transformation.Scale.X, selectedEntity.transformation.Scale.Y);
                            position = new System.Numerics.Vector2(selectedEntity.transformation.Translation.X, selectedEntity.transformation.Translation.Y);
                        }
                        
                    }
                }
                ImGui.EndTable();
            }

            if(selectedComponentIndex >= 0 && selectedComponentIndex < selectedEntity.Components.Count)
            {
                Component selectedComponent = selectedEntity.Components[selectedComponentIndex];
                ImGui.SliderAngle("Rotation", ref rotation);
                selectedComponent.localTransformation.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Backward, rotation);

                if(ImGui.SliderFloat2("Position", ref position, -30, 30))
                {
                    if (!touch)
                    {
                        touch = true;
                        oldPosition = new Vector2(selectedComponent.localTransformation.Translation.X, selectedComponent.localTransformation.Translation.Y);
                    }
                    Vector2 direction = new Vector2(position.X, position.Y);
                    float len = direction.Length();
                    direction.Normalize();
                    direction *= 0.02F;
                    Vector2 result = (new GyroVector(oldPosition) + new GyroVector(direction) * len).vec;
                    selectedComponent.localTransformation.Translation = new Vector2(result.X, result.Y);
                } else if (!ImGui.IsAnyMouseDown())
                {
                    touch = false;
                    position.X = 0;
                    position.Y = 0;
                }
                ImGui.SliderFloat2("Scale", ref scale, -0.1F, 3F);
                selectedComponent.localTransformation.Scale = new Vector2(scale.X, scale.Y);
                if (ImGui.Button("Reset Component"))
                {
                    rotation = 0;
                    position = System.Numerics.Vector2.Zero;
                    selectedComponent.localTransformation.Translation = Vector2.Zero;
                    selectedComponent.localTransformation.Rotation = Quaternion.Identity;
                    selectedComponent.localTransformation.Gyration = Quaternion.Identity;
                    selectedComponent.localTransformation.Scale = Vector2.One;
                }
            }
            else
            {
                ImGui.SliderAngle("Rotation", ref rotation);
                selectedEntity.transformation.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Backward, rotation);
                if(ImGui.SliderFloat2("Position", ref position, -30, 30))
                {
                    if (!touch)
                    {
                        touch = true;
                        oldPosition = new Vector2(selectedEntity.transformation.Translation.X, selectedEntity.transformation.Translation.Y);
                    }
                    Vector2 direction = new Vector2(position.X, position.Y);
                    float len = direction.Length();
                    direction.Normalize();
                    direction *= 0.02F;
                    Vector2 result = (new GyroVector(oldPosition) + new GyroVector(direction) * len).vec;
                    selectedEntity.transformation.Translation = new Vector2(result.X, result.Y);
                } else if (touch && !ImGui.IsAnyMouseDown())
                {
                    touch = false;
                    position.X = 0;
                    position.Y = 0;
                }
                if (ImGui.Button("Reset Entity"))
                {
                    rotation = 0;
                    position = System.Numerics.Vector2.Zero;
                    selectedEntity.transformation.Translation = Vector2.Zero;
                    selectedEntity.transformation.Rotation = Quaternion.Identity;
                    selectedEntity.transformation.Gyration = Quaternion.Identity;
                    selectedEntity.transformation.Scale = Vector2.One;
                }


            }
            ImGui.SliderFloat("Poincare-Klein", ref PK, 0.15F, 2.0F);
            Renderer r = game.scene.Renderer;
            if(r.RenderEffect.Parameters["PK"] != null)
                r.RenderEffect.Parameters["PK"].SetValue(PK);
            if (r.PostProcessEffect.Parameters["PK"] != null)
                r.PostProcessEffect.Parameters["PK"].SetValue(PK);

            ImGui.SliderFloat("Free par", ref freepar, -2, 10);
            r.RenderEffect.Par = freepar;

            if (ImGui.Button("Zapisz poziom"))
            {
                Level l = new Level("test", game.scene, game.finish);
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
