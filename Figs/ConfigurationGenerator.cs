using System;
using System.IO;
using System.Text;

namespace Figs
{
    public class ConfigurationGenerator
    {
        private readonly ILog _log;
        private readonly string _source;
        private readonly string _dest;
        private readonly ConfigurationGeneratorOptions _options;

        public ConfigurationGenerator(ILog log, string source, string dest, ConfigurationGeneratorOptions options)
        {
            _log = log;
            _options = options;
            _source = source;
            _dest = dest;
        }

        public bool Execute()
        {
            _log.WriteMessage($"Using {_options.ConigurationFilePath} for figs configuration source");
            AssertIsSupported();
            WriteEnvironmentConfigurations();
            WriteRuntimeConfiguration();
            return true;
        }

        private void WriteRuntimeConfiguration()
        {
            using (var stream = File.OpenRead(_source))
            {
                var parser = new DefaultConfigurationParser(Encoding.UTF8);
                string contents = parser.Parse(stream, new JsonSettingsProvider(_log, _options.ConigurationFilePath).Load());
                stream.Close();
                File.Delete(_dest);
                File.WriteAllText(_dest, contents);
                _log.WriteMessage($"Generated runtime configuration in {_dest}");
            }
        }

        private void WriteEnvironmentConfigurations()
        {
            foreach (var environment in new JsonSettingsProvider(_log, _options.ConigurationFilePath).LoadAllEnvironments())
            {
                using (var stream = File.OpenRead(_source))
                {
                    var parser = new DefaultConfigurationParser(Encoding.UTF8);
                    string contents = parser.Parse(stream, environment.Value);
                    stream.Close();
                    var output = $"{_dest}.{environment.Key.ToLower()}";
                    File.WriteAllText(output, contents);
                    _log.WriteMessage($"Generated {environment.Key} configuration in {output}");
                }
            }
        }

        private void AssertIsSupported()
        {
            if (_options.SupportedOutputTypes.ContainsKey(_options.OutputType))
            {
                return;
            }
            throw new InvalidOperationException("Output type " + _options.OutputType + " not supported");
        }
    }
}
