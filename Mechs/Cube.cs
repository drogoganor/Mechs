using Mechs.Data;
using System.Numerics;

#nullable disable

namespace Mechs
{
    public enum CubeFace : byte
    {
        Top = 0,
        Bottom = 1,
        Left = 2,
        Right = 3,
        Back = 4,
        Front = 5,
    }

    public static class Cube
    {
        public static VertexPositionTexture[] GetFaceVertices(CubeFace face, Vector3 position, int tex)
        {
            int texture = tex - 1;
            switch (face)
            {
                case CubeFace.Top:
                    return new VertexPositionTexture[]
                    {
                        new VertexPositionTexture(position + new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(1, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, +0.5f, +0.5f), new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, +0.5f, +0.5f), new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(0, 1, texture)),
                    };
                case CubeFace.Bottom:
                    return new VertexPositionTexture[]
                    {
                        new VertexPositionTexture(position + new Vector3(-0.5f,-0.5f, +0.5f),  new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f,-0.5f, +0.5f),  new Vector3(1, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f,-0.5f, -0.5f),  new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f,-0.5f, +0.5f),  new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f,-0.5f, -0.5f),  new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f,-0.5f, -0.5f),  new Vector3(0, 1, texture)),
                    };
                case CubeFace.Left:
                    return new VertexPositionTexture[]
                    {
                        new VertexPositionTexture(position + new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(1, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 1, texture)),
                    };
                case CubeFace.Right:
                    return new VertexPositionTexture[]
                    {
                        new VertexPositionTexture(position + new Vector3(+0.5f, +0.5f, +0.5f), new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(1, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, +0.5f, +0.5f), new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(0, 1, texture)),
                    };
                case CubeFace.Back:
                    return new VertexPositionTexture[]
                    {
                        new VertexPositionTexture(position + new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(1, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(0, 1, texture)),
                    };
                case CubeFace.Front:
                    return new VertexPositionTexture[]
                    {
                        new VertexPositionTexture(position + new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, +0.5f, +0.5f), new Vector3(1, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(0, 0, texture)),
                        new VertexPositionTexture(position + new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(1, 1, texture)),
                        new VertexPositionTexture(position + new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(0, 1, texture)),
                    };
            };

            return null;
        }
    }
}
