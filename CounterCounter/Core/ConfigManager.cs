// CounterCounter/Core/ConfigManager.cs
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using CounterCounter.Models;

namespace CounterCounter.Core
{
    public class ConfigManager
    {
        private const string CONFIG_FILENAME = "config.json";
        private readonly string _configPath;

        public ConfigManager()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string exeDir = Path.GetDirectoryName(exePath) ?? Environment.CurrentDirectory;
            _configPath = Path.Combine(exeDir, CONFIG_FILENAME);
        }

        public CounterSettings Load()
        {
            if (!File.Exists(_configPath))
            {
                Console.WriteLine($"[ConfigManager] Config file not found, creating default: {_configPath}");
                return CounterSettings.CreateDefault();
            }

            try
            {
                string json = File.ReadAllText(_configPath);
                var settings = JsonSerializer.Deserialize<CounterSettings>(json);

                if (settings == null)
                {
                    Console.WriteLine("[ConfigManager] Deserialization returned null, using default");
                    return CounterSettings.CreateDefault();
                }

                Console.WriteLine($"[ConfigManager] Loaded from {_configPath}");
                Console.WriteLine($"[ConfigManager] NextRotationHotkey: {settings.NextRotationHotkey?.GetDisplayText() ?? "null"}");

                return settings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load config: {ex.Message}");
                return CounterSettings.CreateDefault();
            }
        }

        public bool Save(CounterSettings settings)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(_configPath, json);

                Console.WriteLine($"[ConfigManager] Saved to {_configPath}");
                Console.WriteLine($"[ConfigManager] NextRotationHotkey: {settings.NextRotationHotkey?.GetDisplayText() ?? "null"}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save config: {ex.Message}");
                return false;
            }
        }

        public string GetConfigPath()
        {
            return _configPath;
        }

        public bool ConfigExists()
        {
            return File.Exists(_configPath);
        }
    }
}