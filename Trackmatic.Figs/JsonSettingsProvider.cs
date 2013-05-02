using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Trackmatic.Figs
{
    public class JsonSettingsProvider : IProvideSettings
    {
        private readonly string _path;

        public JsonSettingsProvider(string path)
        {
            _path = path;
        }

        public Dictionary<string, string> Load()
        {
            var settings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(_path));
            if (!settings.ContainsKey("configuration") || !settings["configuration"].ContainsKey("active"))
                throw new KeyNotFoundException("An active configuration has not been supplied");
            if (!settings.ContainsKey(settings["configuration"]["active"]))
                throw new KeyNotFoundException(string.Format("Configured configuration {0} could not be found", settings["configuration"]["active"]));
            return settings[settings["configuration"]["active"]];
        }
    }
}