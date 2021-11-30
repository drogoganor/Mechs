using Mechs.Game.Data;
using System.Text.Json;
using Veldrid;
using Veldrid.ImageSharp;

namespace Mechs.Game
{
    public class GameResources
    {
        public TextureFile[] Textures { get; set; }
        private GraphicsDevice graphicsDevice;
        private ResourceFactory resourceFactory;

        public GameResources(GraphicsDevice gd, ResourceFactory rf)
        {
            graphicsDevice = gd;
            resourceFactory = rf;

            var contentDir = Path.Combine(AppContext.BaseDirectory, "Content");
            var textureJsonFilePath = Path.Combine(contentDir, "textures.json");
            var textureJson = File.ReadAllText(textureJsonFilePath);
            var textureFiles = JsonSerializer.Deserialize<TextureFiles>(textureJson);

            foreach (var texture in textureFiles.Textures)
            {
                var textureImage = new ImageSharpTexture(Path.Combine(contentDir, texture.Filename), true);
                var textureResource = textureImage.CreateDeviceTexture(graphicsDevice, resourceFactory);
                var textureView = resourceFactory.CreateTextureView(textureResource);

                texture.Texture = textureResource;
                texture.TextureView = textureView;
            }

            Textures = textureFiles.Textures;
        }
    }
}
