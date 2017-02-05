using System;
using System.IO;
using System.Text;

namespace Figs.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parameters = new Parameters(args);
                parameters.Validate();

                using (var stream = File.OpenRead(parameters.Source.Path))
                {
                    var parser = new DefaultConfigurationParser(Encoding.UTF8);
                    string contents = parser.Parse(stream, new JsonSettingsProvider(new Logger(), parameters.Settings.Path).Load());
                    File.WriteAllText(args[2], contents);
                }
            }
            catch (CliException e)
            {
                Warn("Usage: Figs.Cli.exe \"settings.json\" \"source configuration file\" \"destination configuration file\"");
                Warn(string.Empty);
                Warn(e.Message);
            }
        }

        static void Warn(string message)
        {
            Console.WriteLine(message);
        }

        private class Logger : ILog
        {
            public void WriteMessage(string message)
            {
                Console.WriteLine(message);
            }
        }

        public class Parameters
        {
            private readonly string[] _args;

            public Parameters(string[] args)
            {
                _args = args;
            }

            public SettingsParameter Settings => new SettingsParameter(_args[0]);
            public SourceParameter Source => new SourceParameter(_args[1]);
            public string Dest => _args[2];

            public void Validate()
            {
                if (_args.Length != 3)
                {
                    throw new CliException("Invalid number of arguments supplied");
                }
                Settings.Validate();
                Source.Validate();
            }
        }

        public class SettingsParameter
        {
            public SettingsParameter(string path)
            {
                Path = path;
            }

            public string Path { get; }

            public void Validate()
            {
                if (!File.Exists(Path))
                {
                    throw new CliException($"Settings file could not be found at {Path}");
                }
            }
        }

        public class SourceParameter
        {
            public SourceParameter(string path)
            {
                Path = path;
            }

            public string Path { get; }

            public void Validate()
            {
                if (!File.Exists(Path))
                {
                    throw new CliException($"Source file could not be found at {Path}");
                }
            }
        }

        public class CliException : Exception
        {
            public CliException(string message) : base(message)
            {
                
            }
        }
    }
}
