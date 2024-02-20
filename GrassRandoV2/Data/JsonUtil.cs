using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrassCore;

namespace GrassRando.Data
{
    internal static class JsonUtil
    {
        public static readonly JsonSerializer _js;

        public static T Deserialize<T>(string embeddedResourcePath)
        {
            using StreamReader reader = new StreamReader(typeof(JsonUtil).Assembly.GetManifestResourceStream(embeddedResourcePath));
            using JsonTextReader reader2 = new JsonTextReader(reader);
            return _js.Deserialize<T>(reader2)!;
        }

        static JsonUtil()
        {
            _js = new JsonSerializer
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
            _js.Converters.Add(new StringEnumConverter());
        }
    }

    internal class GrassKeyJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GrassKey);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
