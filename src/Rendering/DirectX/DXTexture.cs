namespace Nine.Graphics.Rendering
{
    using SharpDX.Direct3D12;

    public struct DXTexture
    {
        public readonly Resource Resource;

        //public readonly GpuDescriptorHandle GpuDescriptorHandle;
        //public readonly CpuDescriptorHandle CpuDescriptorHandle;

        public DXTexture(Resource resource)
        {
            this.Resource = resource;
        }

        public static bool operator ==(DXTexture r, DXTexture l)
        {
            return r.Resource == l.Resource;
        }

        public static bool operator !=(DXTexture r, DXTexture l)
        {
            return r.Resource != l.Resource;
        }
    }
}