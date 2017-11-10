
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
        }

        internal static void Update (Dictionary<long, ActualUnit> enemies, Dictionary<long, ActualUnit> vehicles, List<CombatGroup> groups) {
            if (window == null) 
                return;
            
            window.Units = vehicles;
            window.Enemies = enemies;

            window.Groups = groups;
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
            graphics.PreferredBackBufferWidth = 300;
            graphics.PreferredBackBufferHeight = 300;
            cellWidth = graphics.PreferredBackBufferWidth / 1024f * MyStrategy.CellWidth;
        }

        public Dictionary<long, ActualUnit> Enemies { get; internal set; }

        public Dictionary<long, ActualUnit> Units { get; internal set; }
        public List<CombatGroup> Groups { get; internal set; }

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

                

                DrawUnits();

                foreach (var group in Groups) {
                    DrawGrid(group);
                    DrawPoint(group.Goal, Color.Violet);
                    DrawPoint(group.Position, Color.Green);
                }

                spriteBatch.End();

                //UpdateInProcess = false;
            }
            catch (Exception e) {
            }
        }

        protected override void Update (GameTime gameTime) {
            //   base.Update(gameTime);
        }

        private void DrawPoint (Vector point, Color color, float width = 10) {
            var pos = VToScreenV2(point);// new Vector2((float)point.X / 1024f * graphics.PreferredBackBufferWidth, (float)point.Y / 1024f * graphics.PreferredBackBufferHeight) - new Vector2(5, 5);
            DrawPoint(pos, color, width);
        }

        void DrawPoint (Vector2 point, Color color, float width = 10) {

            var pos = point;// ; - new Vector2(width / 2, width / 2);

            Rectangle rect = new Rectangle((int)(pos.X - width / 2), (int)(pos.Y - width / 2), (int)width, (int)width);

            spriteBatch.Draw(
                dummyTexture,
                rect,
                color);
        }

        void DrawPoint (IntVector point, Color color, float width = 10) {
            var pos = VToScreenV2(point);
            DrawPoint(pos, color, width);
        }

        Vector2 VToScreenV2 (IntVector v) {
            return new Vector2(
                v.X / 1024f * graphics.PreferredBackBufferWidth,
                v.Y / 1024f * graphics.PreferredBackBufferHeight);
        }

        Vector2 VToScreenV2 (Vector v) {
            return new Vector2(
                (float)v.X / 1024f * graphics.PreferredBackBufferWidth,
                (float)v.Y / 1024f * graphics.PreferredBackBufferHeight);
        }

        private void DrawGrid (CombatGroup group) {

            if (Groups == null)
                return;
            double max = float.MinValue;
            double min = float.MaxValue;

            foreach (var cell in group.Potentials) {
                if (cell.Value > max)
                    max = cell.Value;
                else if (cell.Value < min)
                    min = cell.Value;
            }
            double delta = max - min;


            foreach (var cell in group.Potentials) {

                float c = (float)((cell.Value - min) / delta);
                gridColor = new Color(1 - c, c, c);

                DrawPoint(cell.Key, gridColor, cellWidth);
                //spriteBatch.Draw(dummyTexture, pos, new Rectangle(0, 0, (int)cellWidth, (int)cellWidth), gridColor);
            }
        }

        private void DrawUnits () {

            foreach (var u in Units.Values)
                DrawPoint(u.Position, Color.Black, 5);
            foreach (var u in Enemies.Values)
                DrawPoint(u.Position, Color.DarkRed, 5);

            //   spriteBatch.Draw(dummyTexture, pos, new Rectangle(0, 0, 5, 5), u.IsMy ? Color.Black : Color.DarkRed);
        }
    }
}


