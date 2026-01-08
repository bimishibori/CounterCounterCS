// CounterCounter/Core/ConfigManager.cs
using System;
using System.IO;
using System.Text.Json;
using CounterCounter.Models;

namespace CounterCounter.Core
{
    public class ConfigManager
    {
        private const string CONFIG_FILENAME = "config.json";
        private const string APP_SETTINGS_FILENAME = "app_settings.json";
        private readonly string _defaultConfigPath;
        private readonly string _appSettingsPath;
        private string _currentConfigPath;

        public string CurrentConfigPath => _currentConfigPath;

        public ConfigManager()
        {
            string exeDir = AppContext.BaseDirectory;
            _defaultConfigPath = Path.Combine(exeDir, CONFIG_FILENAME);
            _appSettingsPath = Path.Combine(exeDir, APP_SETTINGS_FILENAME);
            _currentConfigPath = _defaultConfigPath;
        }

        public AppSettings LoadAppSettings()
        {
            if (!File.Exists(_appSettingsPath))
            {
                return new AppSettings();
            }

            try
            {
                string json = File.ReadAllText(_appSettingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                return settings ?? new AppSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load app settings: {ex.Message}");
                return new AppSettings();
            }
        }

        public bool SaveAppSettings(AppSettings appSettings)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(appSettings, options);
                File.WriteAllText(_appSettingsPath, json);
                Console.WriteLine($"[ConfigManager] App settings saved: LastOpenedFilePath={appSettings.LastOpenedFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save app settings: {ex.Message}");
                return false;
            }
        }

        public CounterSettings Load()
        {
            return LoadFromFile(_currentConfigPath);
        }

        public CounterSettings LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[ConfigManager] Config file not found, creating default: {filePath}");
                return CounterSettings.CreateDefault();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var settings = JsonSerializer.Deserialize<CounterSettings>(json);

                if (settings == null)
                {
                    Console.WriteLine("[ConfigManager] Deserialization returned null, using default");
                    return CounterSettings.CreateDefault();
                }

                _currentConfigPath = filePath;
                Console.WriteLine($"[ConfigManager] Loaded from {filePath}");
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
            return SaveToFile(settings, _currentConfigPath);
        }

        public bool SaveToFile(CounterSettings settings, string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(filePath, json);

                _currentConfigPath = filePath;
                Console.WriteLine($"[ConfigManager] Saved to {filePath}");
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
            return _currentConfigPath;
        }

        public string GetDefaultConfigPath()
        {
            return _defaultConfigPath;
        }

        public bool ConfigExists()
        {
            return File.Exists(_currentConfigPath);
        }

        public bool ConfigExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void ResetToDefault()
        {
            _currentConfigPath = _defaultConfigPath;
        }
    }
}