using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LabProject3
{
    public static class JsonUtils
    {
        public static bool IsJsonEmpty(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Console.WriteLine("The JSON file is empty.");
                return true;
            }
            return false;
        }

        public static List<UniversityClass> Deserialize(string filePath)
        {
            string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

            if (IsJsonEmpty(json))
                return new List<UniversityClass>();

            var classes = JsonSerializer.Deserialize<List<UniversityClass>>(json);
            return classes;
        }

        public static void Serialize(string filePath, List<UniversityClass> classes)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(classes, options);
            File.WriteAllText(filePath, json);
        }
    }
}
