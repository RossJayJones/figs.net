using System;
using System.IO;
using System.Text;

namespace Figs
{
    public class ConfigurationGenerator
    {
        private readonly ILog _log;
        private readonly PathFactory _pathFactory;
        private readonly ConfigurationGeneratorOptions _options;

        public ConfigurationGenerator(ILog log, PathFactory pathFactory, ConfigurationGeneratorOptions options)
        {
            _log = log;
            _options = options;
            _pathFactory = pathFactory;
        }

        public bool Execute()
        {
            _log.WriteMessage($"Using {_options.ConfigurationFilePath} for figs configuration source");
            PrepareDirectories();
            AssertIsSupported();
            WriteOriginalConfiguration();
            WriteEnvironmentConfigurations();
            WriteRuntimeConfiguration();
            return true;
        }

        private void WriteRuntimeConfiguration()
        {
            using (var stream = File.OpenRead(_pathFactory.SourcePath()))
            {
                var parser = new DefaultConfigurationParser(Encoding.UTF8);
                string contents = parser.Parse(stream, new JsonSettingsProvider(_log, _options.ConfigurationFilePath).Load());
                stream.Close();
                File.Delete(_pathFactory.DestPath());
                File.WriteAllText(_pathFactory.DestPath(), contents);
                _log.WriteMessage($"Generated runtime configuration in {_pathFactory.DestPath()}");
            }
        }

        private void WriteOriginalConfiguration()
        {
            var output = _pathFactory.FigsPath("config");
            File.WriteAllText(output, File.ReadAllText(_pathFactory.SourcePath()));
            _log.WriteMessage($"Generated original configuration in {output}");
        }

        private void WriteEnvironmentConfigurations()
        {
            foreach (var environment in new JsonSettingsProvider(_log, _options.ConfigurationFilePath).LoadAllEnvironments())
            {
                using (var stream = File.OpenRead(_pathFactory.SourcePath()))
                {
                    var parser = new DefaultConfigurationParser(Encoding.UTF8);
                    string contents = parser.Parse(stream, environment.Value);
                    stream.Close();
                    var output = _pathFactory.FigsPath(environment.Key.ToLower());
                    File.WriteAllText(output, contents);
                    _log.WriteMessage($"Generated {environment.Key} configuration in {output}");
                }
            }
        }

        public void PrepareDirectories()
        {
            if (Directory.Exists(_pathFactory.FigsDiretory()))
            {
                return;
            }
            Directory.CreateDirectory(_pathFactory.FigsDiretory());
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
