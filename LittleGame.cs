using System;
using System.Collections.Generic;
using LittleCoins.Things;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleCoins
{
    public enum Aspect
    {
        Keep = 0,
        Stretch = 1,
    }

    public class LittleGame : IDisposable
    {
        public Point Resolution { get; set; } = new Point(800, 600);
        public Aspect Aspect { get; set; } = Aspect.Keep;
        public int FPS { get; set; } = 60;
        public bool FullScreen { get; set; } = false;
        public DisplayOrientation Orientation { get; set; } = DisplayOrientation.Default;

        public bool PixelPerfect
        {
            get => MonoGame.PixelPerfect;
            set => MonoGame.PixelPerfect = value;
        }

        public bool AllowResizing
        {
            get => MonoGame.Window.AllowUserResizing;
            set => MonoGame.Window.AllowUserResizing = value;
        }

        // -----------------------------------------------

        public static LittleGame Current { get; private set; }

#if DEBUG
        public static bool Debug => true;
#else
        public static bool Debug => false;
#endif

        public string RootPath => Debug ? "../../Assets" : "Assets";
        public LittleMonoGame MonoGame { get; }
        public GraphicsDevice GraphicsDevice => MonoGame.GraphicsDevice;

        private readonly Stack<Scene> scenes;

        public LittleGame()
        {
            scenes = new Stack<Scene>();
            MonoGame = new LittleMonoGame(scenes.Peek);
            Current = this;
        }

        private void WindowChange(object sender = null, EventArgs e = null)
        {
            var bounds = MonoGame.Window.ClientBounds;
            var scaleX = bounds.Width / (float)Resolution.X;
            var scaleY = bounds.Height / (float)Resolution.Y;

            Console.WriteLine(Resolution);
            Console.WriteLine(bounds);

            if (Aspect == Aspect.Keep)
            {
                scaleX = scaleY = Math.Min(scaleX, scaleY);
                var width = Convert.ToInt32(Resolution.X * scaleX);
                var height = Convert.ToInt32(Resolution.Y * scaleY);
                var x = (bounds.Width - width) / 2;
                var y = (bounds.Height - height) / 2;
                var vp = new Viewport(x, y, width, height);
                MonoGame.GraphicsDevice.Viewport = vp;
            }

            MonoGame.SetScale(scaleX, scaleY);
        }

        public void Run(Scene scene)
        {

            MonoGame.Window.ClientSizeChanged += WindowChange;
            MonoGame.Graphics.SupportedOrientations = Orientation;
            MonoGame.Graphics.IsFullScreen = FullScreen;
            var windowRes = Resolution;
            if (FullScreen)
            {
                var dm = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                windowRes = new Point(dm.Width, dm.Height);
            }
            MonoGame.Graphics.PreferredBackBufferWidth = windowRes.X;
            MonoGame.Graphics.PreferredBackBufferHeight = windowRes.Y;
            MonoGame.Graphics.ApplyChanges();

            MonoGame.Window.AllowAltF4 = true;

            MonoGame.IsFixedTimeStep = true;
            MonoGame.TargetElapsedTime = TimeSpan.FromSeconds(1d / FPS);

            scenes.Push(scene);
            MonoGame.Run();
        }

        public void Dispose()
        {
            MonoGame?.Dispose();
        }
    }
}
