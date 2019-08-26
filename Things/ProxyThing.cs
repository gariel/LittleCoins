using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleCoins.Things
{
    public abstract class ProxyThing : DrawableThing
    {
        protected readonly DrawableThing Child;

        public override Vector2 Position
        {
            get => Child.Position;
            set => Child.Position = value;
        }

        public override Rectangle Rect => Child.Rect;

        protected ProxyThing(DrawableThing child)
        {
            Child = child;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Child.Draw(spriteBatch);
        }

        public override void Update(float delta)
        {
            Child.Update(delta);
        }

        public override float Left
        {
            get => Child.Left;
            set => Child.Left = value;
        }

        public override float Top
        {
            get => Child.Top;
            set => Child.Top = value;
        }

        public override float Right
        {
            get => Child.Right;
            set => Child.Right = value;
        }

        public override float Bottom
        {
            get => Child.Bottom;
            set => Child.Bottom = value;
        }
    }
}
