using Common.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialColor.Tests
{
    [TestFixture]
    public class JsonTests
    {
        [Test(ExpectedResult = "C:\\JsonTests\\test")]
        public string Path()
        {
            var info = new FileInfo("C:\\JsonTests\\test");
            return info.FullName;
        }

        [Test]
        public void Write_Basic()
        {
            var serializer = Newtonsoft.Json.JsonSerializer.Create();

            using (var textWriter = new StreamWriter("C:\\JsonTests\\result.json"))
            {
                using (var jsonWriter = new Newtonsoft.Json.JsonTextWriter(textWriter))
                {
                    serializer.Serialize(jsonWriter, new ElementColorInfo(new Color32Multiplier(0.1f, 0.2f, 0.3f), 0.6f));
                    jsonWriter.Close();
                }
                textWriter.Close();
            }
        }

        [Test]
        public void Write_Dictionary()
        {
            var serializer = Newtonsoft.Json.JsonSerializer.Create();

            var dictionary = new Dictionary<SimHashes, ElementColorInfo>
            {
                { SimHashes.ToxicSand, new ElementColorInfo(new Color32Multiplier(0.9f, 0.8f, 0.7f), 0.4f) },
                { SimHashes.Aerogel, new ElementColorInfo(new Color32Multiplier(0.6f, 0.5f, 0.4f), 0.3f) },
            };

            using (var textWriter = new StreamWriter("C:\\JsonTests\\result_dictionary.json"))
            {
                using (var jsonWriter = new Newtonsoft.Json.JsonTextWriter(textWriter))
                {
                    serializer.Serialize(jsonWriter, dictionary);
                    jsonWriter.Close();
                }
                textWriter.Close();
            }
        }

        [TestCase("result.json")]
        public void Read(string path)
        {
            ElementColorInfo result;
            var serializer = Newtonsoft.Json.JsonSerializer.Create();

            using (var textReader = new StreamReader(path))
            {
                using (var jsonReader = new Newtonsoft.Json.JsonTextReader(textReader))
                {
                    result = serializer.Deserialize<ElementColorInfo>(jsonReader);

                    jsonReader.Close();
                }
                textReader.Close();
            }
        }

        [TestCase("D:\\Downloads\\Games\\Oxygen Not Included\\221865\\modding\\MaterialColor\\DummyInjectionTarget\\bin\\Debug\\ElementsColorInfo.json")]
        public void Read_Dictionary(string path)
        {

            Dictionary<SimHashes, ElementColorInfo> result;
            var serializer = Newtonsoft.Json.JsonSerializer.Create();

            using (var textReader = new StreamReader(path))
            {
                using (var jsonReader = new Newtonsoft.Json.JsonTextReader(textReader))
                {
                    result = serializer.Deserialize<Dictionary<SimHashes, ElementColorInfo>>(jsonReader);

                    jsonReader.Close();
                }
                textReader.Close();
            }
        }

        //[TestCase("D:\\Downloads\\Games\\Oxygen Not Included\\221865\\modding\\MaterialColor\\TypeColors.json")]
        //public void ExportTypeColors(string path)
        //{
        //    MaterialColorGuard.ExportTypeColorsDictionaryToJson(path);
        //}
    }
}
