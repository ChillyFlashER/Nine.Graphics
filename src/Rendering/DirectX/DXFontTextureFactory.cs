namespace Nine.Graphics.Rendering
{
    using System;
    using Nine.Graphics.Content;
    using SharpDX.Direct3D12;

    public class DXFontTextureFactory : FontTextureFactory<Resource>
    {
        public DXFontTextureFactory(IFontLoader loader) : base(loader) { }

        protected override Texture<Resource> Create8BppTexture(int width, int height, byte[] pixels)
        {
            throw new NotImplementedException();
        }

        protected override void Update8bppTexture(Resource texture, int width, int height, byte[] pixels)
        {
            throw new NotImplementedException();
        }
    }
}
