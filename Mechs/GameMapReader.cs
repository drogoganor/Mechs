using Mechs.Data;
using System.Text.Json;

#nullable disable

namespace Mechs
{
    public class GameMapReader
    {
        private GameMap _gameMap;
        public GameMapReader(string filename)
        {
            var contentDir = Path.Combine(AppContext.BaseDirectory, "Content");
            var mapJsonFilePath = Path.Combine(contentDir, filename);
            var mapJsonText = File.ReadAllText(mapJsonFilePath);
            _gameMap = JsonSerializer.Deserialize<GameMap>(mapJsonText);
        }

        public VertexPositionTexture[] GetVertexArray()
        {
            var vertexList = new List<VertexPositionTexture>();
            foreach (var block in _gameMap.Blocks)
            {
                var faceIndex = 0;
                foreach (var faceTexture in block.FaceTextures)
                {
                    if (faceTexture != 0)
                    {
                        vertexList.AddRange(Cube.GetFaceVertices((CubeFace)faceIndex, block.Position, faceTexture));
                    }

                    faceIndex++;
                }
            }

            return vertexList.ToArray();
        }
    }
}
