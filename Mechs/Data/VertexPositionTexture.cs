using System.Numerics;

namespace Mechs.Data
{
    public struct VertexPositionTexture
    {
        public const uint SizeInBytes = 24;

        public float PosX;
        public float PosY;
        public float PosZ;

        public float TexU;
        public float TexV;
        public float TexW;

        public VertexPositionTexture(Vector3 pos, Vector3 uvw)
        {
            PosX = pos.X;
            PosY = pos.Y;
            PosZ = pos.Z;
            TexU = uvw.X;
            TexV = uvw.Y;
            TexW = uvw.Z;
        }
    }
}
