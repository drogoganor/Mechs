using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mechs.Game.Data
{
    public class JsonConverterVector3 : JsonConverter<Vector3>
    {
        public override Vector3 Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var allValuesRead = false;
            var valueArray = new float[3];
            var index = 0;
            while (!allValuesRead && reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.EndArray:
                        allValuesRead = true;
                        break;
                    case JsonTokenType.Number:
                        valueArray[index] = reader.GetInt32();
                        break;
                }

                index++;
            }

            return new Vector3(valueArray[0], valueArray[1], valueArray[2]);
        }

        public override void Write(
            Utf8JsonWriter writer,
            Vector3 value,
            JsonSerializerOptions options)
        {

        }
    }
}
