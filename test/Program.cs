namespace Nine.Graphics
{
    using System;
    using System.Numerics;

    public class Program
    {
        private static readonly TextureId[] Textures =
        {
            "https://avatars0.githubusercontent.com/u/511355?v=3&s=460",
            "Content/Logo.png",
        };

        static DrawingContext drawingContext;

        const int Count = 48;
        static Sprite[] Sprites = new Sprite[Count + 1];
        static float offset = 0;

        static readonly float spriteSize = 32;
        static readonly float scale = 18;
        static readonly float piOver4 = (float)Math.PI / (Count / 2);

        public static void Main(string[] args)
        {
            drawingContext = DrawingContext.CreateOpenGL(1024, 600);
            //drawingContext = DrawingContext.CreateDirectX(1024, 600);

            while (drawingContext.Host.DrawFrame(Draw)) { }
        }

        private static void Draw(int width, int height)
        {
            float xoffset = (width / 2) - (spriteSize / 2);
            float yoffset = (height / 2) - (spriteSize / 2);

            offset += 0.01f;
            for (int i = 0; i < Count; i++)
            {
                var t = offset + piOver4 * i;
                var x = (float)(16 * Math.Pow(Math.Sin(t), 3)) * scale + xoffset;
                var y = (float)(13 * Math.Cos(t) - 5 * Math.Cos(2 * t) - 2 * Math.Cos(3 * t) - Math.Cos(4 * t)) * scale - yoffset;
                Sprites[i] = new Sprite(Textures[0], size: new Vector2(spriteSize), position: new Vector2(x, -y));
            }

            Sprites[Count] = new Sprite(Textures[1], size: new Vector2(spriteSize * 3), position: new Vector2(xoffset - spriteSize, yoffset - spriteSize + 20));

            var camera = Matrix4x4.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);

            drawingContext.SpriteRenderer.Draw(camera, Sprites);
        }
    }
}
