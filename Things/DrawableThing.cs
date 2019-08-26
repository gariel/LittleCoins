using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleCoins.Things
{
    public abstract class DrawableThing : ILittleThing
    {
        public virtual Vector2 Position { get; set; }
        public virtual bool Visible { get; set; } = true;

        public abstract Rectangle Rect { get; }

        public abstract float Left { get; set; }
        public abstract float Top { get; set; }

        public virtual float Right
        {
            get => Left + Rect.Width;
            set => Left = value - Rect.Width;
        }

        public virtual float Bottom
        {
            get => Top + Rect.Height;
            set => Top = value - Rect.Height;
        }

        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void Update(float delta)
        {
        }
    }
}
