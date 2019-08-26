using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleCoins.Things
{
    public abstract class Layer : DrawableThing
    {
        protected readonly List<ILittleThing> Things = new List<ILittleThing>();
        protected internal bool Built { get; set; } = false;

        public override Rectangle Rect { get; } = new Rectangle(Point.Zero, LittleGame.Current.Resolution);

        public abstract void Build();

        public override void Update(float delta)
        {
            Things.ForEach(t => t.Update(delta));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var thing in Things)
            {
                if (thing is DrawableThing drawable)
                {
                    drawable.Draw(spriteBatch);
                }
            }
        }

        public virtual void Add(ILittleThing littleThing)
        {
            Things.Add(littleThing);
        }

        // TODO: implement layer positioning
        public override float Left { get; set; }
        public override float Top { get; set; }
    }
}
