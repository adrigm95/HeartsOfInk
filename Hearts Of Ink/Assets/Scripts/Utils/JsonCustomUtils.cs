using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public static class JsonCustomUtils<T>
    {
        public static T ReadObjectFromFile(string path)
        {
            string fileContent;
            T readedObject;

            fileContent = File.ReadAllText(path);
            readedObject = JsonConvert.DeserializeObject<T>(fileContent);

            return readedObject;
        }

        public static void SaveObjectIntoFile(T content, string path)
        {
            string fileContent = JsonConvert.SerializeObject(content);

            File.WriteAllText(path, fileContent);
        }
    }
}
