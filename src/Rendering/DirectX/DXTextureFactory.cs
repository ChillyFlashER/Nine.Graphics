namespace Nine.Graphics.Rendering
{
    using System.Runtime.InteropServices;
    using Nine.Graphics.Content;
    using SharpDX.Direct3D12;
    using SharpDX.DXGI;

    public class DXTextureFactory : TextureFactory<DXTexture>
    {
        private readonly DXGraphicsHost graphicsHost;

        public DXTextureFactory(DXGraphicsHost host, ITextureLoader loader, int capacity = 1024)
            : base(loader, capacity)
        {
            this.graphicsHost = host;
        }

        public override Texture<DXTexture> CreateTexture(TextureContent data)
        {
            DXDebug.CheckAccess(graphicsHost.Device);

            SharpDX.Direct3D12.Resource texture = null;

            var textureDesc = ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, data.Width, data.Height);
            texture = graphicsHost.Device.CreateCommittedResource(new HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination);

            long uploadBufferSize = GetRequiredIntermediateSize(texture, 0, 1);

            // Create the GPU upload buffer.
            var textureUploadHeap = graphicsHost.Device.CreateCommittedResource(new HeapProperties(CpuPageProperty.WriteBack, MemoryPool.L0), HeapFlags.None, 
                ResourceDescription.Texture2D(Format.R8G8B8A8_UNorm, data.Width, data.Height), ResourceStates.GenericRead);

            // Copy data to the intermediate upload heap and then schedule a copy 
            // from the upload heap to the Texture2D.
            var handle = GCHandle.Alloc(data.Pixels, GCHandleType.Pinned);
            var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(data.Pixels, 0);
            textureUploadHeap.WriteToSubresource(0, null, ptr, (sizeof(byte) * 4) * data.Width, data.Pixels.Length);
            handle.Free();

            // TODO: Runtime invalid call error below 

            //var commandList = graphicsHost.RequestCommandList();
            //commandList.CopyTextureRegion(new TextureCopyLocation(texture, 0), 0, 0, 0, new TextureCopyLocation(textureUploadHeap, 0), null);
            //commandList.ResourceBarrierTransition(texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource);

            //// Describe and create a SRV for the texture.
            //var srvDesc = new ShaderResourceViewDescription
            //{
            //    Shader4ComponentMapping = DXHelper.DefaultComponentMapping(),
            //    Format = textureDesc.Format,
            //    Dimension = ShaderResourceViewDimension.Texture2D,
            //    Texture2D = { MipLevels = 1 },
            //};

            //graphicsHost.Device.CreateShaderResourceView(texture, srvDesc, graphicsHost.SRVHeap.CPUDescriptorHandleForHeapStart);

            //// Command lists are created in the recording state, but there is nothing
            //// to record yet. The main loop expects it to be closed, so close it now.
            //commandList.Close();

            //graphicsHost.CommandQueue.ExecuteCommandList(commandList);

            textureUploadHeap.Dispose();

            return new Texture<DXTexture>(new DXTexture(texture), data.Width, data.Height, data.Left, data.Right, data.Top, data.Bottom, data.IsTransparent);
        }

        private long GetRequiredIntermediateSize(SharpDX.Direct3D12.Resource destinationResource, int firstSubresource, int NumSubresources)
        {
            var desc = destinationResource.Description;
            long requiredSize;
            graphicsHost.Device.GetCopyableFootprints(ref desc, firstSubresource, NumSubresources, 0, null, null, null, out requiredSize);
            return requiredSize;
        }
    }
}
