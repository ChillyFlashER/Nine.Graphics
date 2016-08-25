namespace Nine.Graphics.Rendering
{
    using System.Numerics;

    // Visual Studio Graphics Analyzer: float2 x4byte float2
    public struct Vertex2D
    {
        public Vector2 Position;
        public int Color;
        public Vector2 TextureCoordinate;

        public const int SizeInBytes = 8 + 4 + 8;
    }
}
