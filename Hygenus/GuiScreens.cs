using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGui.Standard;

namespace Hygenus
{
    public class GuiScreens
    {
        public delegate void ShowScreen(GameTime gameTime);

        public ShowScreen CurrentScreen { get; private set; }

        Hygenus game;
        private ImGUIRenderer GuiRenderer;
        private GraphicsDeviceManager graphics;

        private ImGuiWindowFlags utilityFlags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize;

        private System.Numerics.Vector2 mainMenuSize;
        private System.Numerics.Vector2 mainMenuPos;

        

        public GuiScreens(Hygenus game, GraphicsDeviceManager graphics, ImGUIRenderer GuiRenderer)
        {
            this.game = game;
            this.GuiRenderer = GuiRenderer;
            this.graphics = graphics;
            CurrentScreen = MainMenu;
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
            ImGui.BeginTable("levels", 1);
            if (ImGui.BeginTable("table1", 3))
            {
                for (int row = 0; row < 4; row++)
                {
                    ImGui.TableNextRow();
                    for (int column = 0; column < 3; column++)
                    {
                        ImGui.TableSetColumnIndex(column);
                        ImGui.Text($"Row {row} Column {column}");
                    }
                }
                ImGui.EndTable();
            }
            ImGui.EndTable();
            ImGui.End();
            GuiRenderer.EndLayout();

        }
    }
}
