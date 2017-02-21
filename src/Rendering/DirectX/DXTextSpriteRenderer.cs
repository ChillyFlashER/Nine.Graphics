namespace Nine.Graphics.Rendering
{
    using System;
    using SharpDX.Direct3D12;

    public class DXTextSpriteRenderer : TextSpriteRenderer<SharpDX.Direct3D12.Resource>
    {
        public DXTextSpriteRenderer(DXFontTextureFactory textureFactory) 
            : base(textureFactory)
        {
        }
    }
}
