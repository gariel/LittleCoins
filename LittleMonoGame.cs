using System;
using LittleCoins.Things;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleCoins
{
    public class LittleMonoGame : Game
    {
        private readonly Func<Scene> getScene;
        private SpriteBatch spriteBatch;

        public GraphicsDeviceManager Graphics { get; }
        public bool PixelPerfect { get; set; } = true;

        public Matrix? ScaleMatrix = null;

        public LittleMonoGame(Func<Scene> getScene)
        {
            this.getScene = getScene;
            Graphics =  new GraphicsDeviceManager(this)
            {
                PreferMultiSampling = true,
            };
        }

        public void SetScale(float scaleX, float scaleY)
        {
            ScaleMatrix = Matrix.CreateScale(new Vector3(scaleX, scaleY, 1));
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

//        protected override void UnloadContent()
//        {
//        }

        protected override void Update(GameTime gameTime)
        {
            var scene = getScene();
            if (!scene.Built)
            {
                scene.Build();
                scene.RunToDos();
                foreach (var layer in scene.Layers)
                {
                    if (!layer.Built)
                    {
                        layer.Build();
                        layer.Built = true;
                    }
                }
                scene.Built = true;
            }

            base.Update(gameTime);
            scene.Update(gameTime.ElapsedGameTime.Milliseconds / 1000f);
        }

        protected override void Draw(GameTime gameTime)
        {
            var scene = getScene();

            var sampleState = PixelPerfect ? SamplerState.PointWrap : SamplerState.LinearClamp;

            GraphicsDevice.Clear(scene.BackColor);

            spriteBatch.Begin(samplerState: sampleState, transformMatrix: ScaleMatrix);
            scene.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);

        }
    }
}
