using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace LittleCoins.Things
{
    public interface ITexturesProvider
    {
        Texture2D[] Textures { get; }
    }

    public class TextureListProvider : ITexturesProvider
    {
        public Texture2D[] Textures { get; }

        public TextureListProvider(params Texture2D[] textures)
        {
            Textures = textures;
        }
    }

    public class TextureKeysProvider : TextureListProvider
    {
        public TextureKeysProvider(IAsset<Texture2D> asset, params string[] keys) :
            base(keys.Select(k => asset.Load(k)).ToArray())
        {
        }
    }
}
