
using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    static class Render {

        static List<ActualUnit> units = new List<ActualUnit>();

        public static void Update (List<ActualUnit> units) {
            Render.units = units;
        }

        public static void Run () {
            using (var game = new GameWindow()) {
                game.Load += (sender, e) => {
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;
                };

                game.Resize += (sender, e) => {
                    GL.Viewport(0, 0, 1024, 1024);
                };

                game.UpdateFrame += (sender, e) => {


                    // add game logic, input handling
                    if (game.Keyboard[Key.Escape]) {
                        game.Exit();
                    }
                };

                game.RenderFrame += (sender, e) => {
                    // render graphics
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

                    GL.Begin(PrimitiveType.Triangles);

                    GL.Color3(Color.MidnightBlue);
                    //GL.Vertex2(-1.0f, 1.0f);
                    //GL.Color3(Color.SpringGreen);
                    //GL.Vertex2(0.0f, -1.0f);
                    //GL.Color3(Color.Ivory);
                    //GL.Vertex2(1.0f, 1.0f);

                    foreach (var u in units)
                        GL.Rect(u.Position.X, u.Position.Y, u.Position.X+1,u.Position.Y+1);

                    GL.End();

                    game.SwapBuffers();
                };

                // Run the game at 60 updates per second
                game.Run(60.0);
            }
        }
    }
}


