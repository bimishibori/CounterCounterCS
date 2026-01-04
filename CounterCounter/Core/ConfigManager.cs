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
                return CounterSettings.CreateDefault();
            }

            try
            {
                string json = File.ReadAllText(_configPath);
                var settings = JsonSerializer.Deserialize<CounterSettings>(json);
                return settings ?? CounterSettings.CreateDefault();
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