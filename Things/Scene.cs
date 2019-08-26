using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LittleCoins.Things
{
    public class Scene : Layer
    {
        public virtual Color BackColor => Color.Black;
        public List<Layer> Layers { get; } = new List<Layer>();
        private List<Action> ToDo { get; } = new List<Action>();

        public override void Build() { }

        public override void Update(float delta)
        {
            RunToDos();
            Layers.ForEach(l => l.Update(delta));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Layers.ForEach(l => l.Draw(spriteBatch));
        }

        public void AddLayer(Layer layer)
        {
            ToDo.Add(() => Layers.Add(layer));
        }

        public void ClearLayers()
        {
            ToDo.Add(() => Layers.Clear() );
        }

        protected internal void RunToDos()
        {
            ToDo.ForEach(a => a());
            ToDo.Clear();
        }
    }
}
