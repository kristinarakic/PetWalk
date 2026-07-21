using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace PetWalk.Services
{
    public class SerializationService
    {
        public void ExportToJson<T>(List<T> data, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
        }

        public List<T> ImportFromJson<T>(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            string json = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<List<T>>(json, options) ?? new List<T>();
        }

        public void ExportToXml<T>(List<T> data, string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, data);
            }
        }

        public List<T> ImportFromXml<T>(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var serializer = new XmlSerializer(typeof(List<T>));
            using (var reader = new StreamReader(filePath))
            {
                return (List<T>?)serializer.Deserialize(reader) ?? new List<T>();
            }
        }
    }
}
