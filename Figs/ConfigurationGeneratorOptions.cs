using System.Collections.Generic;

namespace Figs
{
    public class ConfigurationGeneratorOptions
    {
        public string ConigurationFilePath { get; set; }

        public string OutputType { get; set; }

        public string ProjectName { get; set; }

        public IDictionary<string, string> SupportedOutputTypes { get; set; } 
    }
}
