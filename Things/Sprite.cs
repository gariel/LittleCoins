using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleCoins.Things
{
    public class Sprite : DrawableThing
    {
        public virtual Texture2D Texture { get; set; }
        public virtual bool InvertX { get; set; } = false;
        public virtual bool InvertY { get; set; } = false;
        public virtual Color Modulate { get; set; } = Color.White;
        public virtual float Rotation { get; set; } = 0f;
        public virtual bool CenteredOrigin { get; set; } = false;
        public virtual Vector2 Scale { get; set; } = Vector2.One;
        public virtual Vector2 Origin => CenteredOrigin ? ZeroRect.Center.ToVector2() : Vector2.Zero;

        protected Rectangle ZeroRect => Texture?.Bounds ?? Rectangle.Empty;

        public override Rectangle Rect
        {
            get
            {
                var pos = Position.ToPoint();
                var size = ZeroRect.Size;

                if (Scale != Vector2.One)
                {
                    size = (size.ToVector2() * Scale).ToPoint();
                }

                if (CenteredOrigin)
                {
                    pos = new Point(pos.X - size.X / 2, pos.Y - size.Y / 2);
                }

                return new Rectangle(pos, size);
            }
        }

        public override float Left
        {
            get => Position.X - (CenteredOrigin ? ZeroRect.Width / 2f : 0f);
            set => Position = new Vector2(value + (CenteredOrigin ? ZeroRect.Width / 2f : 0f), Position.Y);
        }

        public override float Top
        {
            get => Position.Y - (CenteredOrigin ? ZeroRect.Height / 2f : 0f);
            set => Position = new Vector2(Position.X, value + (CenteredOrigin ? ZeroRect.Height / 2f : 0f));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible || Texture == null)
            {
                return;
            }

            var flip = SpriteEffects.None;
            if (InvertX) flip |= SpriteEffects.FlipHorizontally;
            if (InvertY) flip |= SpriteEffects.FlipVertically;

            spriteBatch.Draw(
                texture: Texture,
                destinationRectangle: Rect,
                sourceRectangle: ZeroRect,
                color: Modulate,
                rotation: MathHelper.ToRadians(Rotation),
                origin: Origin,
                effects: flip,
                layerDepth: 0f
            );
        }
    }

    public interface IPausable
    {
        bool Paused { get; set; }
    }

    public class AnimatedSprite : Sprite, IPausable
    {
        private ITexturesProvider textureProvider;
        public ITexturesProvider TextureProvider
        {
            get => textureProvider;
            set
            {
                textureProvider = value;
                TextureIndex = 0;
            }
        }

        private float counter = 0;

        public int TextureIndex { get; private set; } = 0;
        public int FramesPerSecond { get; set; } = 10;
        public bool Paused { get; set; }

        public override Texture2D Texture
        {
            get => TextureProvider?.Textures[TextureIndex];
            set { }
        }


        public AnimatedSprite(ITexturesProvider textureProvider)
        {
            TextureProvider = textureProvider;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            if (!Paused)
            {
                counter += delta;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (TextureProvider == null)
            {
                return;
            }

            if (counter >= 1f / FramesPerSecond)
            {
                counter = 0;
                TextureIndex = (TextureIndex + 1) % TextureProvider.Textures.Length;
            }

            base.Draw(spriteBatch);
        }
    }

    public class ScrollBackground : Sprite, IPausable
    {
        public Vector2 Scroll { get; private set; }
        public Vector2 Speed { get; set; }
        public bool Paused { get; set; }

        public override void Update(float delta)
        {
            if (Paused)
            {
                return;
            }

            var oldScroll = Scroll;
            Scroll = new Vector2(
                (oldScroll.X + (Speed.X * delta)) % Texture.Width,
                (oldScroll.Y + (Speed.Y * delta)) % Texture.Height);

            var leftX = ((int)Scroll.X) - ((int)oldScroll.X);
            var rightX = Texture.Width - leftX;
            if (leftX < 0)
            {
                rightX = Math.Abs(leftX);
                leftX = Texture.Width - rightX;
            }
            if (leftX > 0)
            {
                var leftSize = leftX * Texture.Height;
                var rightSize = (Texture.Width - leftX) * Texture.Height;
                var left = new Color[leftSize];
                var right = new Color[rightSize];

                Texture.GetData(0, new Rectangle(0, 0, leftX, Texture.Height), left, 0, leftSize);
                Texture.GetData(0, new Rectangle(leftX, 0, rightX, Texture.Height), right, 0, rightSize);

                Texture.SetData(0, new Rectangle(0, 0, rightX, Texture.Height), right, 0, rightSize);
                Texture.SetData(0, new Rectangle(rightX, 0, leftX, Texture.Height), left, 0, leftSize);
            }

            var topY = ((int)Scroll.Y) - ((int)oldScroll.Y);
            var bottomY = Texture.Height - topY;
            if (topY < 0)
            {
                bottomY = Math.Abs(topY);
                topY = Texture.Height - bottomY;
            }

            if (topY > 0)
            {
                var topSize = topY * Texture.Width;
                var bottomSize = (Texture.Height - topY) * Texture.Width;
                var top = new Color[topSize];
                var bottom = new Color[bottomSize];

                Texture.GetData(0, new Rectangle(0, 0, Texture.Width, topY), top, 0, topSize);
                Texture.GetData(0, new Rectangle(0, topY, Texture.Width, bottomY), bottom, 0, bottomSize);

                Texture.SetData(0, new Rectangle(0, 0, Texture.Width, bottomY), bottom, 0, bottomSize);
                Texture.SetData(0, new Rectangle(0, bottomY, Texture.Width, topY), top, 0, topSize);
            }

            base.Update(delta);
        }
    }
}
