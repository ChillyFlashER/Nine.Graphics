namespace Nine.Graphics.Rendering
{
    using SharpDX.Direct3D12;
    using System;
    using System.Diagnostics;
    using Factory4 = SharpDX.DXGI.Factory4;

    // https://msdn.microsoft.com/en-us/library/windows/desktop/bb509553(v=vs.85).aspx
    enum DXErrorCode : uint
    {
        DXGI_ERROR_DEVICE_HUNG = 0x887A0006,
        DXGI_ERROR_DEVICE_REMOVED = 0x887A0005,
        DXGI_ERROR_DEVICE_RESET = 0x887A0007,
        DXGI_ERROR_DRIVER_INTERNAL_ERROR = 0x887A0020,
        DXGI_ERROR_FRAME_STATISTICS_DISJOINT = 0x887A000B,
        DXGI_ERROR_GRAPHICS_VIDPN_SOURCE_IN_USE = 0x887A000C,
        DXGI_ERROR_INVALID_CALL = 0x887A0001,
        DXGI_ERROR_MORE_DATA = 0x887A0003,
        DXGI_ERROR_NONEXCLUSIVE = 0x887A0021,
        DXGI_ERROR_NOT_CURRENTLY_AVAILABLE = 0x887A0022,
        DXGI_ERROR_NOT_FOUND = 0x887A0002,
        DXGI_ERROR_REMOTE_CLIENT_DISCONNECTED = 0x887A0023,
        DXGI_ERROR_REMOTE_OUTOFMEMORY = 0x887A0024,
        DXGI_ERROR_WAS_STILL_DRAWING = 0x887A000A,
        DXGI_ERROR_UNSUPPORTED = 0x887A0004,
        DXGI_ERROR_ACCESS_LOST = 0x887A0026,
        DXGI_ERROR_WAIT_TIMEOUT = 0x887A0027,
        DXGI_ERROR_SESSION_DISCONNECTED = 0x887A0028,
        DXGI_ERROR_RESTRICT_TO_OUTPUT_STALE = 0x887A0029,
        DXGI_ERROR_CANNOT_PROTECT_CONTENT = 0x887A002A,
        DXGI_ERROR_ACCESS_DENIED = 0x887A002B,
        DXGI_ERROR_NAME_ALREADY_EXISTS = 0x887A002C,
        DXGI_ERROR_SDK_COMPONENT_MISSING = 0x887A002D,
    }

    static class DXDebug
    {
        [Conditional("DEBUG")]
        public static void CheckAccess(Device device)
        {
            if (device.DeviceRemovedReason.Failure)
            {
                var error = (DXErrorCode)device.DeviceRemovedReason.Code;
                throw new InvalidOperationException($"DirectX Device was removed ({error})");
            }
        }

        public static bool ValidateDevice(Device device)
        {
            // The D3D Device is no longer valid if the default adapter changed since the device
            // was created or if the device has been removed.

            // First, get the LUID for the adapter from when the device was created.
            var previousAdapterLuid = device.AdapterLuid;

            // Next, get the information for the current default adapter.
            using (var factory = new Factory4())
            {
                var currentDefaultAdapter = factory.Adapters[0];
                var currentDesc = currentDefaultAdapter.Description;

                // If the adapter LUIDs don't match, or if the device reports that it has been removed,
                // a new D3D device must be created.
                if (previousAdapterLuid != currentDesc.Luid ||
                    device.DeviceRemovedReason.Failure)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
