
using System;

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    static class Render {
        private static MonoRenderer window;




        public static void Run () {
            window = new MonoRenderer();
            window.Run();

            // main.Run();
        }

        

        internal static void Update (Dictionary<long, ActualUnit> enemies, Dictionary<long, ActualUnit> vehicles, CombatGroup helics) {
            if (window == null)
                return;
            window.Units = vehicles;
            window.Enemies = enemies;
            
            window.Helics = helics;
            window.CustomUpdate();
        }
    }

    class MonoRenderer: Game {
        Texture2D dummyTexture;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        float cellWidth;
        public MonoRenderer () {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 512;
            graphics.PreferredBackBufferHeight = 512;
            cellWidth = graphics.PreferredBackBufferWidth / 1024f * MyStrategy.CellWidth;
        }

        public Dictionary<long, ActualUnit> Enemies { get; internal set; }
       
        public Dictionary<long, ActualUnit> Units { get; internal set; }
        public CombatGroup Helics { get; internal set; }

        protected override void Initialize () {
            base.Initialize();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            dummyTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });

            Units = new Dictionary<long, ActualUnit>();
            Units.Add(1, new ActualUnit(new Model.Vehicle(1, 100, 100, 3, 1, 100, 100, 1, 60, 60 * 60, 60, 60 * 60, 60, 60 * 60, 60, 60, 60, 60, 6, 1, Model.VehicleType.Tank, false, false, new int[] { })));
        }

        
        Color gridColor = new Color(Color.Black, 2.5f);

        public void CustomUpdate () {
            try {

                GraphicsDevice.Clear(Color.White);

                spriteBatch.Begin(SpriteSortMode.Deferred,
                  BlendState.AlphaBlend,
                  SamplerState.PointClamp,
                  null, null, null, null);




                DrawGrid();

                foreach (var u in Units.Values)
                    DrawUnit(u);
                foreach (var u in Enemies.Values)
                    DrawUnit(u);

                DrawPoint(Helics.Goal, Color.Violet);

                DrawPoint(Helics.Position, Color.Green);

                spriteBatch.End();

                //UpdateInProcess = false;
            }
            catch (Exception e) {
            }
        }

        protected override void Update (GameTime gameTime) {
         //   base.Update(gameTime);
        }

        private void DrawPoint (Vector point, Color color) {
            var pos = new Vector2((float)point.X / 1024f * graphics.PreferredBackBufferWidth, (float)point.Y / 1024f * graphics.PreferredBackBufferHeight) - new Vector2(5,5);
            spriteBatch.Draw(dummyTexture, pos, new Rectangle(0, 0, 10, 10), color);
        }

        private void DrawGrid () {

            if (Helics == null)
                return;
            double max = float.MinValue;
            double min = float.MaxValue;

            foreach (var cell in Helics.Potentials) {
                if (cell.Value > max)
                        max = cell.Value;
                    else if (cell.Value < min)
                        min = cell.Value;
                }
            double delta = max - min;


            foreach (var cell in Helics.Potentials) {
                var pos3 = new Vector2(cell.Key.X , cell.Key.Y );

                float c = (float)((cell.Value - min) / delta);
                gridColor = new Color(1 - c, c, c);
                spriteBatch.Draw(dummyTexture, pos3, new Rectangle(0, 0, (int)cellWidth, (int)cellWidth), gridColor);
            }
        }

        private void DrawUnit (ActualUnit u) {
            Vector2 pos = new Vector2((float)u.Position.X - 2.5f, (float)u.Position.Y - 2.5f);
            pos = new Vector2(pos.X / 1024f * graphics.PreferredBackBufferWidth, pos.Y / 1024f * graphics.PreferredBackBufferHeight);
            spriteBatch.Draw(dummyTexture, pos, new Rectangle(0, 0, 5, 5), u.IsMy? Color.Black : Color.DarkRed);
        }
    }
}


