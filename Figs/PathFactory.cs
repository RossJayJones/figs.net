using System;
using System.IO;

namespace Figs
{
    public abstract class PathFactory
    {
        protected readonly ConfigurationGeneratorOptions Options;

        protected PathFactory(ConfigurationGeneratorOptions options)
        {
            Options = options;
        }

        public abstract string SourcePath();
        public abstract string DestPath();
        public abstract string FigsPath(string environment);
        public abstract string FigsDiretory();
    }

    public class WebPathFactory : PathFactory
    {
        public WebPathFactory(ConfigurationGeneratorOptions options) : base(options)
        {
        }

        public override string SourcePath()
        {
            return $".\\{Options.SupportedOutputTypes[Options.OutputType]}.figs.config";
        }

        public override string DestPath()
        {
            return $".\\{Options.SupportedOutputTypes[Options.OutputType]}.config";
        }

        public override string FigsPath(string environment)
        {
            return $".\\.figs\\{Options.SupportedOutputTypes[Options.OutputType]}.{environment}";
        }

        public override string FigsDiretory()
        {
            return ".\\.figs";
        }
    }

    public class AppPathFactory : PathFactory
    {
        public AppPathFactory(ConfigurationGeneratorOptions options) : base(options)
        {
        }

        public override string SourcePath()
        {
            return Path.Combine(@".\", Options.BinPath, $"{Options.ProjectName}.{Extension(Options)}.config");
        }

        public override string DestPath()
        {
            return Path.Combine(@".\", Options.BinPath, $"{Options.ProjectName}.{Extension(Options)}.config");
        }

        public override string FigsPath(string environment)
        {
            return Path.Combine(@".\", Options.BinPath, ".figs", $"{Options.ProjectName}.{Extension(Options)}.{environment}");
        }

        public override string FigsDiretory()
        {
            return Path.Combine(@".\", Options.BinPath, ".figs");
        }

        private string Extension(ConfigurationGeneratorOptions options)
        {
            if (!options.SupportedOutputTypes.ContainsKey(Options.OutputType))
            {
                throw new InvalidOperationException("Output type " + Options.OutputType + " not supported");
            }
            return options.SupportedOutputTypes[Options.OutputType];
        }
    }
}
