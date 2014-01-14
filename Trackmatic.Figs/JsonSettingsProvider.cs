using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Trackmatic.Figs
{
    public class JsonSettingsProvider : IProvideSettings
    {
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
                Log("Loading active configuration from .figs file");
                configuration = File.ReadAllText(figs);
            }
            else
            {
                Log("Loading active configuration from .json file");
                if (!settings.ContainsKey("configuration") || !settings["configuration"].ContainsKey("active"))
                    throw new KeyNotFoundException("An active configuration has not been supplied");
                configuration = settings["configuration"]["active"];
            }

            if (!settings.ContainsKey(configuration))
                throw new KeyNotFoundException(string.Format("Configured configuration {0} could not be found", settings["configuration"]["active"]));
            return settings[configuration];
        }

        private void Log(string message)
        {
            if (_log == null) return;
            _log.LogMessage(message);
        }
    }
}