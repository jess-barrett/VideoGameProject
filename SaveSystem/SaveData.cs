﻿using System;
using System.IO;
using System.Text.Json;

namespace GameProject2.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public int CoinCount { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public float PlayerX { get; set; }
        public float PlayerY { get; set; }
        public float MusicVolume { get; set; }
        public float SfxVolume { get; set; }

        public static void Save(SaveData data, string filename = "savegame.json")
        {
            try
            {
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filename, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save failed: {ex.Message}");
            }
        }

        public static SaveData Load(string filename = "savegame.json")
        {
            try
            {
                if (File.Exists(filename))
                {
                    string json = File.ReadAllText(filename);
                    return JsonSerializer.Deserialize<SaveData>(json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load failed: {ex.Message}");
            }

            return null;
        }

        public static bool SaveExists(string filename = "savegame.json")
        {
            return File.Exists(filename);
        }
    }
}