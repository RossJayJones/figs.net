﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Figs
{
    public class FigsBuildTask : Task, ILog
    {
        [Required]
        public string ConigurationFilePath { get; set; }

        [Required]
        public string OutputType { get; set; }

        [Required]
        public string ProjectName { get; set; }

        public string BinPath { get; set; }

        public override bool Execute()
        {
            WriteMessage($"Using {ConigurationFilePath} for figs configuration source");
            var generator = CreateConfigurationGenerator();
            return generator.Execute();
        }

        public void WriteMessage(string message)
        {
            Log.LogMessage(MessageImportance.High, $"[Figs] {message}");
        }

        private bool IsWeb()
        {
            return File.Exists(".\\Web.config");
        }

        private ConfigurationGenerator CreateConfigurationGenerator()
        {
            return IsWeb() ? Web() : App();
        }

        private ConfigurationGenerator Web()
        {
            var options = new ConfigurationGeneratorOptions
            {
                ConfigurationFilePath = ConigurationFilePath,
                OutputType = OutputType,
                ProjectName = ProjectName,
                BinPath = BinPath,
                SupportedOutputTypes = new Dictionary<string, string>
                {
                    {"Library", "Web"},
                    {"Exe", "App"}
                }
            };
            var source = $".\\{options.SupportedOutputTypes[OutputType]}.figs.config";
            WriteMessage($"Loading configuration from {source}");
            var dest = $".\\{options.SupportedOutputTypes[OutputType]}.config";
            WriteMessage($"Writing configuration to {dest}");
            var factory = new WebPathFactory(options);
            return new ConfigurationGenerator(this, factory, options);
        }

        private ConfigurationGenerator App()
        {
            var options = new ConfigurationGeneratorOptions
            {
                ConfigurationFilePath = ConigurationFilePath,
                OutputType = OutputType,
                ProjectName = ProjectName,
                BinPath = BinPath,
                SupportedOutputTypes = new Dictionary<string, string>
                {
                    {"Library", "dll"},
                    {"Exe", "exe"}
                }
            };
            var source = Path.Combine(@".\", BinPath, $"{ProjectName}.{Extension(options)}.config");
            WriteMessage($"Loading configuration from {source}");
            WriteMessage($"Writing configuration to {source}");
            var factory = new AppPathFactory(options);
            return new ConfigurationGenerator(this, factory, options);
        }

        private string Extension(ConfigurationGeneratorOptions options)
        {
            if (!options.SupportedOutputTypes.ContainsKey(OutputType))
            {
                throw new InvalidOperationException("Output type " + OutputType + " not supported");
            }
            return options.SupportedOutputTypes[OutputType];
        }
    }
}