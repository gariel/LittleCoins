using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace LittleCoins
{
    public static class Assets
    {
        private static ImageAssets imageInstance;
        public static ImageAssets Image => imageInstance ?? (imageInstance = new ImageAssets());
    }

    public interface IAsset<T>
    {
        T Load(params string[] parts);
        AssetGroup<T> Group(params string[] parts);
    }

    public abstract class BaseAssets<T> : IAsset<T>
    {
        protected abstract string AssetsFolder { get; }
        protected abstract string FileExtension { get; }

        private readonly Type cacheKey = typeof(T);

        private static Dictionary<Type, Dictionary<string, T>> cache
            = new Dictionary<Type, Dictionary<string, T>>();

        protected BaseAssets()
        {
            if (!cache.ContainsKey(cacheKey))
            {
                cache[cacheKey] = new Dictionary<string, T>();
            }
        }

        private Stream LoadStream(string path)
        {
            var root = LittleGame.Current.RootPath;
            var complete = $"{root}/{AssetsFolder}/{path}.{FileExtension}";
            return File.OpenRead(complete);
        }

        public T Load(params string[] parts)
        {
            var path = Path.Combine(parts);
            if (cache[cacheKey].ContainsKey(path))
            {
                return cache[cacheKey][path];
            }

            var stream = LoadStream(path);
            var texture = CreateFromStream(stream);
            cache[cacheKey][path] = texture;
            return texture;
        }

        public AssetGroup<T> Group(params string[] parts)
        {
            return new AssetGroup<T>(this, parts);
        }

        protected abstract T CreateFromStream(Stream stream);
    }

    public class AssetGroup<T> : IAsset<T>
    {
        private readonly BaseAssets<T> baseAssets;
        private readonly string[] basePath;

        internal AssetGroup(BaseAssets<T> baseAssets, string[] basePath)
        {
            this.baseAssets = baseAssets;
            this.basePath = basePath;
        }

        public T Load(params string[] parts)
        {
            var path = basePath.Concat(parts).ToArray();
            return baseAssets.Load(path);
        }

        public AssetGroup<T> Group(params string[] parts)
        {
            var path = basePath.Concat(parts).ToArray();
            return new AssetGroup<T>(baseAssets, path);
        }
    }

    public class ImageAssets : BaseAssets<Texture2D>
    {
        protected override string AssetsFolder => "Images";
        protected override string FileExtension => "png";

        protected override Texture2D CreateFromStream(Stream stream)
        {
            return Texture2D.FromStream(LittleGame.Current.GraphicsDevice, stream);
        }
    }
}
