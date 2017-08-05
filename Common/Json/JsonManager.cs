using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Json
{
    public class JsonManager
    {
        public JsonSerializer Serializer = JsonSerializer.CreateDefault();

        public T Deserialize<T>(string path)
        {
            T result;

            using (var streamReader = new StreamReader(path))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    result = Serializer.Deserialize<T>(jsonReader);

                    jsonReader.Close();
                }
                streamReader.Close();
            }

            return result;
        }

        public void Serialize(object value, string path)
        {
            using (var streamReader = new StreamWriter(path))
            {
                using (var jsonReader = new JsonTextWriter(streamReader))
                {
                    Serializer.Serialize(jsonReader, value);

                    jsonReader.Close();
                }
                streamReader.Close();
            }
        }
    }
}
