using System.Text.Json.Serialization;
using Veldrid;

#nullable disable

namespace Mechs.Data
{
    public class TextureFile
    {
        public int ID { get; set; }
        public string Filename { get; set; }

        [JsonIgnore]
        public Texture Texture { get; set; }

        [JsonIgnore]
        public TextureView TextureView { get; set; }
    }

    public class TextureFiles
    {
        public TextureFile[] Textures { get; set; }
    }
}
