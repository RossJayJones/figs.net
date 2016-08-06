using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Figs
{
    public class JsonSettingsProvider : IProvideSettings
    {
        private const string ConfigurationKey = "configuration";
        private const string ActiveKey = "active";

        private readonly ILog _log;
        private readonly string _path;

        public JsonSettingsProvider(string path)
            : this(null, path)
        {
            
        }

        public JsonSettingsProvider(ILog log, string path)
        {
            _log = log;
            _path = path;
        }

        public Dictionary<string, string> Load()
        {
            var settings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(_path));
            string configuration;
            const string figs = "../.figs";
            if (File.Exists(figs))
            {
                _log?.WriteMessage("Loading active configuration from .figs file");
                configuration = File.ReadAllText(figs);
            }
            else
            {
                _log?.WriteMessage("Loading active configuration from .json file");
                if (!settings.ContainsKey(ConfigurationKey) || !settings[ConfigurationKey].ContainsKey(ActiveKey))
                {
                    throw new KeyNotFoundException("An active configuration has not been supplied");
                }
                configuration = settings[ConfigurationKey][ActiveKey];
            }

            if (!settings.ContainsKey(configuration))
                throw new KeyNotFoundException($"Configured configuration {settings[ConfigurationKey][ActiveKey]} could not be found");
            return settings[configuration];
        }

        public Dictionary<string, Dictionary<string, string>> LoadAllEnvironments()
        {
            var settings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(_path));
            if (settings.ContainsKey(ConfigurationKey))
            {
                settings.Remove(ConfigurationKey);
            }
            return settings;
        }
    }
}