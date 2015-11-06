﻿#if DX
namespace Nine.Graphics.DirectX
#else
namespace Nine.Graphics.OpenGL
#endif
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Nine.Graphics.Content;
    using Rendering;

    public partial class TextureFactory : ITexturePreloader
    {
        enum LoadState { None, Loading, Loaded, Failed, Missing }

        struct Entry
        {
            public LoadState LoadState;
            public Texture Slice;

            public override string ToString() => $"{ LoadState }: { Slice }";
        }

        private readonly SynchronizationContext syncContext = SynchronizationContext.Current;
        private readonly IGraphicsHost graphicsHost;
        private readonly ITextureLoader loader;
        private Entry[] textures;

        public TextureFactory(IGraphicsHost graphicsHost, ITextureLoader loader, int capacity = 1024)
        {
            if (graphicsHost == null) throw new ArgumentNullException(nameof(graphicsHost));
            if (loader == null) throw new ArgumentNullException(nameof(loader));

            this.graphicsHost = graphicsHost;
            this.loader = loader;
            this.textures = new Entry[capacity];
        }

        public Texture GetTexture(TextureId textureId)
        {
            if (textureId.Id <= 0) return null;

            if (textures.Length <= textureId.Id)
            {
                Array.Resize(ref textures, MathHelper.NextPowerOfTwo(TextureId.Count));
            }

            var entry = textures[textureId.Id];
            if (entry.LoadState == LoadState.None)
            {
                LoadTexture(textureId);

                // Ensures the method returns a valid result when the
                // async load method succeeded synchroniously.
                entry = textures[textureId.Id];
            }

            return entry.LoadState != LoadState.None ? entry.Slice : null;
        }

        private async Task LoadTexture(TextureId textureId)
        {
            try
            {
                textures[textureId.Id].LoadState = LoadState.Loading;

                var data = await loader.Load(textureId.Name);
                if (data == null)
                {
                    await LoadTexture(TextureId.Missing);
                    textures[textureId.Id].Slice = textures[TextureId.Missing.Id].Slice;
                    textures[textureId.Id].LoadState = LoadState.Missing;
                    return;
                }

                textures[textureId.Id].Slice = PlatformCreateTexture(data);
                textures[textureId.Id].LoadState = LoadState.Loaded;
            }
            catch
            {
                await LoadTexture(TextureId.Error);
                textures[textureId.Id].Slice = textures[TextureId.Error.Id].Slice;
                textures[textureId.Id].LoadState = LoadState.Failed;
            }
        }

        Task ITexturePreloader.Preload(params TextureId[] textures)
        {
            if (textures.Length <= 0) return Task.FromResult(0);
            if (syncContext == null) throw new ArgumentNullException(nameof(SynchronizationContext));

            var tcs = new TaskCompletionSource<int>();

            syncContext.Post(async _ =>
            {
                var maxId = textures.Max(texture => texture.Id);
                if (this.textures.Length <= maxId)
                {
                    Array.Resize(ref this.textures, MathHelper.NextPowerOfTwo(TextureId.Count));
                }

                await Task.WhenAll(textures
                    .Where(textureId => this.textures[textureId.Id].LoadState == LoadState.None)
                    .Select(LoadTexture));

                tcs.SetResult(0);

            }, null);

            return tcs.Task;
        }
    }
}
