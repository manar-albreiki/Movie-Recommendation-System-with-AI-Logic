using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MovieRecommendationSystem.Utilities
{
    public static class FileManager
    {
        // =========================================
        // Load Data from JSON File (SAFE VERSION)
        // =========================================
        public static List<T> LoadData<T>(string filePath)
        {
            try
            {
                // If file does not exist → return empty list
                if (!File.Exists(filePath))
                {
                    return new List<T>();
                }

                string json = File.ReadAllText(filePath);

                // If file is empty → return empty list
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<T>();
                }

                // Convert JSON safely
                var data = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // If deserialization fails → return empty list instead of crash
                return data ?? new List<T>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine("JSON Format Error: " + ex.Message);
                return new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading file: " + ex.Message);
                return new List<T>();
            }
        }

        // =========================================
        // Save Data to JSON File
        // =========================================
        public static void SaveData<T>(string filePath, List<T> data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving file: " + ex.Message);
            }
        }
    }
}