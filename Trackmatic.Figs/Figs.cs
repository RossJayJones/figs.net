﻿using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Trackmatic.Figs
{

    public class Figs : Task, ILog
    {
        private static readonly Dictionary<string, string> OutputTypes;

        static Figs()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    "Library",
                    "dll"
                },
                { 
                    "Exe",
                    "exe"
                }
            };
            OutputTypes = dictionary;
        }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Using " + ConigurationFilePath + " for figs configuration source");
            string path = Path.Combine(@".\", BinPath, $"{ProjectName}.{Extension()}.config");
            Log.LogMessage(MessageImportance.High, "Using " + path + " for figs configuration target");
            using (FileStream stream = File.OpenRead(path))
            {
                string contents = new DefaultConfigurationParser(Encoding.UTF8).Parse(stream, new JsonSettingsProvider(this, ConigurationFilePath).Load());
                stream.Close();
                File.Delete(path);
                File.WriteAllText(path, contents);
            }
            return true;
        }

        private string Extension()
        {
            if (!OutputTypes.ContainsKey(OutputType))
            {
                throw new InvalidOperationException("Output type " + OutputType + " not supported");
            }
            return OutputTypes[OutputType];
        }

        public void LogMessage(string message)
        {
            Log.LogMessage(MessageImportance.High, message);
        }

        [Required]
        public string BinPath { get; set; }

        [Required]
        public string ConigurationFilePath { get; set; }

        [Required]
        public string OutputType { get; set; }

        [Required]
        public string ProjectName { get; set; }
    }
}

