using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrassCore;
using UnityEngine;

namespace GrassRando.Data
{
    internal static class JsonUtil
    {
        public static readonly JsonSerializer _js;

        public static T Deserialize<T>(string embeddedResourcePath)
        {
            using JsonTextReader reader = new(new StreamReader(typeof(JsonUtil).Assembly.GetManifestResourceStream(embeddedResourcePath)));
            return _js.Deserialize<T>(reader)!;
        }

        public static T DeserializeFile<T>(string path)
        {
            using JsonTextReader reader = new(new StreamReader(Path.Combine(Application.persistentDataPath, $"{path}")));
            return _js.Deserialize<T>(reader)!;
        }

        public static void Serialize<T>(T obj, string filePath)
        {
            using JsonTextWriter writer = new(new StreamWriter(Path.Combine(Application.persistentDataPath, $"{filePath}")));
            _js.Serialize(writer, obj);
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
}
