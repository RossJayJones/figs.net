using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Trackmatic.Figs.Tests
{
    public class configuration_parser_tests
    {
        [Test]
        public void symbols_in_a_config_file_are_replaced_with_values_from_figs_config()
        {
            using (var stream = File.OpenRead("sample.config"))
            {
                var parser = new DefaultConfigurationParser(Encoding.ASCII);
                var provider = new JsonSettingsProvider("settings.json");
                var settings = provider.Load();
                var result = parser.Parse(stream, settings);
                Assert.AreEqual(File.ReadAllText("expected.config"), result);
            }
        }

        [Test]
        public void an_exception_is_thrown_when_key_missing_from_figs_config()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                {
                    using (var stream = File.OpenRead("missing.sample.config"))
                    {
                        var parser = new DefaultConfigurationParser(Encoding.ASCII);
                        var provider = new JsonSettingsProvider("settings.json");
                        var settings = provider.Load();
                        parser.Parse(stream, settings);
                    }
                });
        }
    }
}