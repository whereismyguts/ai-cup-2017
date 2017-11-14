
using System;

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

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
        internal static void Update (List<Command> commands) {
            if (window == null)
                return;
            window.Commands = commands;
        }

        internal static void Update (List<Cluster> clusters) {
            if (window == null)
                return;
            window.Clusters = clusters;
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
        public List<Cluster> Clusters { get; internal set; }

        protected override void Initialize () {
            base.Initialize();

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            dummyTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });

            Units = new Dictionary<long, ActualUnit>();
            Units.Add(1, new ActualUnit(new Model.Vehicle(1, 100, 100, 3, 1, 100, 100, 1, 60, 60 * 60, 60, 60 * 60, 60, 60 * 60, 60, 60, 60, 60, 6, 1, Model.VehicleType.Tank, false, false, new int[] { })));
        }


        Color gridColor = new Color(Color.Black, 2.5f);
        internal List<Command> Commands;

        public void CustomUpdate () {
            try {
                if (GraphicsDevice == null || spriteBatch == null)
                    return;

                GraphicsDevice.Clear(Color.White);

                spriteBatch.Begin(SpriteSortMode.Deferred,
                  BlendState.AlphaBlend,
                  SamplerState.PointClamp,
                  null, null, null, null);




                foreach (var group in Groups) {
                    //   DrawGrid(group);
                    DrawLine(VToScreenV2( group.Goal), VToScreenV2(group.Position), 3, Color.Violet);
                }

                DrawUnits();

                //DrawCommands();
                DrawClusters();

                spriteBatch.End();

                //UpdateInProcess = false;
            }
            catch (Exception e) {
            }
        }

        private void DrawClusters () {
            foreach (var c in Clusters) {


                Vector2[] vertex = new Vector2[] {
                    VToScreenV2(new Vector(c.PositionX - c.Radius, c.PositionY - c.Radius)),
                    VToScreenV2(new Vector(c.PositionX - c.Radius, c.PositionY + c.Radius)),
                    VToScreenV2(new Vector(c.PositionX + c.Radius, c.PositionY + c.Radius)),
                    VToScreenV2(new Vector(c.PositionX + c.Radius, c.PositionY - c.Radius)) };

                DrawPolygon(vertex, 4, Color.Blue);
            }
        }

        public void DrawPolygon (Vector2[] vertex, int count, Color color) {
            if (count > 0) {
                for (int i = 0; i < count - 1; i++) {
                    DrawLine(vertex[i], vertex[i + 1], 1, color);
                }
                DrawLine(vertex[count - 1], vertex[0], 1, color);
            }
        }



        protected override void Update (GameTime gameTime) {
            //   base.Update(gameTime);
        }

        private void DrawPoint (Vector point, Color color, float width = 10) {
            var pos = VToScreenV2(point);// new Vector2((float)point.X / 1024f * graphics.PreferredBackBufferWidth, (float)point.Y / 1024f * graphics.PreferredBackBufferHeight) - new Vector2(5, 5);
            DrawPoint(pos, color, width);
        }


        void DrawPixel (double x, double y, Color color) {

            spriteBatch.Draw(dummyTexture,
                new Rectangle((int)x, (int)y, 1, 1),
               null,
                color, //colour of line
                0, //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);
        }

        public void DrawLine (Vector2 point1, Vector2 point2, int lineWidth, Color color) {

            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            spriteBatch.Draw(dummyTexture, point1, null, color,
            angle, Vector2.Zero, new Vector2(length, lineWidth),
            SpriteEffects.None, 0f);
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

        void DrawCommands () {
            foreach (var c in Commands) {
                MoveCommand move = c as MoveCommand;

                if (move != null) {
                    var g = Groups.FirstOrDefault(gr => gr.Id == move.GroupId);
                    if (g != null)
                        DrawLine(VToScreenV2(g.Position), VToScreenV2(new Vector(g.Position.X + move.X, g.Position.Y + move.Y)), 3, Color.LightGreen);
                }
            }

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
                gridColor = new Color(1 - c, c, 0);

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


