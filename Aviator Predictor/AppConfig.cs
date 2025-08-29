using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Aviator_Hack
{
    public class AppConfig
    {
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AviatorPredictor",
            "config.json");

        public class PredictionSettings
        {
            public int MaxHistorySize { get; set; } = 100;
            public double ConfidenceThreshold { get; set; } = 0.7;
            public bool EnablePatternAnalysis { get; set; } = true;
            public bool EnableTrendAnalysis { get; set; } = true;
            public int MinDataPointsForAnalysis { get; set; } = 5;
        }

        public class UISettings
        {
            public bool EnableAnimations { get; set; } = true;
            public bool ShowDetailedAnalysis { get; set; } = true;
            public string Theme { get; set; } = "Default";
            public bool RememberWindowPosition { get; set; } = true;
        }

        public class SecuritySettings
        {
            public bool EnablePasswordHashing { get; set; } = true;
            public int MaxLoginAttempts { get; set; } = 3;
            public int SessionTimeoutMinutes { get; set; } = 30;
        }

        public PredictionSettings Prediction { get; set; } = new PredictionSettings();
        public UISettings UI { get; set; } = new UISettings();
        public SecuritySettings Security { get; set; } = new SecuritySettings();

        private static AppConfig? _instance;
        private static readonly object _lock = new object();

        public static AppConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = Load();
                        }
                    }
                }
                return _instance;
            }
        }

        private AppConfig() { }

        public static AppConfig Load()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string jsonContent = File.ReadAllText(ConfigFilePath);
                    var config = JsonSerializer.Deserialize<AppConfig>(jsonContent);
                    return config ?? new AppConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
            }

            return new AppConfig();
        }

        public void Save()
        {
            try
            {
                string? configDir = Path.GetDirectoryName(ConfigFilePath);
                if (!string.IsNullOrEmpty(configDir) && !Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonContent = JsonSerializer.Serialize(this, options);
                File.WriteAllText(ConfigFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving config: {ex.Message}");
            }
        }

        public void ResetToDefaults()
        {
            Prediction = new PredictionSettings();
            UI = new UISettings();
            Security = new SecuritySettings();
            Save();
        }

        public Dictionary<string, object> GetConfigSummary()
        {
            return new Dictionary<string, object>
            {
                ["ConfigFile"] = ConfigFilePath,
                ["PredictionEnabled"] = Prediction.EnablePatternAnalysis,
                ["MaxHistory"] = Prediction.MaxHistorySize,
                ["Theme"] = UI.Theme,
                ["SecurityLevel"] = Security.EnablePasswordHashing ? "High" : "Low"
            };
        }
    }
}
