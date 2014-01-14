using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Trackmatic.Figs
{
    public class Figs : Task, ILog
    {
        [Required]
        public string BinPath { get; set; }

        [Required]
        public string ProjectName { get; set; }

        [Required]
        public string ConigurationFilePath { get; set; }

        [Required]
        public string OutputType { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Using " + ConigurationFilePath + " for figs configuration source");
            var file = Path.Combine(".\\", BinPath, string.Format("{0}.{1}.config", ProjectName, Extension()));
            Log.LogMessage(MessageImportance.High, "Using " + file + " for figs configuration target");
            using (var stream = File.OpenRead(file))
            {
                var parser = new DefaultConfigurationParser(Encoding.UTF8);
                var settings = new JsonSettingsProvider(this, ConigurationFilePath);
                var config = parser.Parse(stream, settings.Load());
                stream.Close();
                File.Delete(file);
                File.WriteAllText(file, config);
            }
            return true;
        }

        private string Extension()
        {
            if (!OutputTypes.ContainsKey(OutputType))
                throw new InvalidOperationException("Output type " + OutputType + " not supported");
            return OutputTypes[OutputType];
        }

        public void LogMessage(string message)
        {
            Log.LogMessage(MessageImportance.High, message);
        }

        private static Dictionary<string, string> OutputTypes = new Dictionary<string, string>
            {
                {"Library", "dll"},
                {"Exe", "exe"}
            };
    }
}
