using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Trackmatic.Figs
{

    public class FigsAlongSide : Task, ILog
    {
        private static readonly Dictionary<string, string> OutputTypes;

        static FigsAlongSide()
        {
            var dictionary = new Dictionary<string, string>
            {
                {
                    "Library",
                    "Web"
                },
                {
                    "Exe",
                    "App"
                }
            };
            OutputTypes = dictionary;
        }

        private string ConfigFile()
        {
            if (!OutputTypes.ContainsKey(OutputType))
            {
                throw new InvalidOperationException("Output type " + OutputType + " not supported");
            }
            return $".\\{OutputTypes[OutputType]}.config";
        }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Using " + ConigurationFilePath + " for figs configuration source");
            Log.LogMessage(MessageImportance.High, "Using " + FigsFile() + " for figs configuration target");
            using (FileStream stream = File.OpenRead(FigsFile()))
            {
                string contents = new DefaultConfigurationParser(Encoding.UTF8).Parse(stream, new JsonSettingsProvider(this, ConigurationFilePath).Load());
                stream.Close();
                File.Delete(ConfigFile());
                File.WriteAllText(ConfigFile(), contents);
            }
            return true;
        }

        private string FigsFile()
        {
            if (!OutputTypes.ContainsKey(OutputType))
            {
                throw new InvalidOperationException("Output type " + OutputType + " not supported");
            }
            return $".\\{OutputTypes[OutputType]}.figs.config";
        }

        public void LogMessage(string message)
        {
            Log.LogMessage(MessageImportance.High, message);
        }

        [Required]
        public string ConigurationFilePath { get; set; }

        [Required]
        public string OutputType { get; set; }

        [Required]
        public string ProjectName { get; set; }
    }
}

