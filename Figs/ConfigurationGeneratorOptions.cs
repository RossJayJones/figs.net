using System.Collections.Generic;

namespace Figs
{
    public class ConfigurationGeneratorOptions
    {
        public string ConfigurationFilePath { get; set; }

        public string OutputType { get; set; }

        public string ProjectName { get; set; }

        public string BinPath { get; set; }

        public IDictionary<string, string> SupportedOutputTypes { get; set; } 
    }
}
