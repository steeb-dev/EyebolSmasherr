using Newtonsoft.Json.Old;
using System;
using UnityEngine;

// This file contains converters for UnityEngine.Vector{2,3,4} types, as they cause some
// serialization problems if they go through Json.NET reflection based conversion.

namespace EXPToolkit.Serializers.JsonNet
{
    public class TextureConverter : JsonConverter
    {
        [JsonObject(MemberSerialization.OptIn)]
        private struct TextureRaw
        {
            [JsonProperty]
            public byte[] data;

            [JsonProperty]
            public int width;

            [JsonProperty]
            public int height;

            [JsonProperty]
            public bool mipMap;

            [JsonProperty]
            public TextureFormat format;                       
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Texture2D) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.Null) return null;

            var texData = serializer.Deserialize<TextureRaw>(reader);
            Texture2D returnVal = new Texture2D(texData.width, texData.height, texData.format, texData.mipMap);
            returnVal.LoadImage(texData.data);
            return returnVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var texture2D = (Texture2D)value;
            var texData = new TextureRaw();

            texData.width = texture2D.width;
            texData.height = texture2D.height;
            texData.format = texture2D.format;
            texData.mipMap = false;
            texData.data = texture2D.EncodeToPNG();
            serializer.Serialize(writer, texData);
        }
    }
}