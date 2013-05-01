using System.Collections.Generic;
using NUnit.Framework;

namespace Trackmatic.Figs.Tests
{
    public class json_settings_provider_tests
    {
        [Test]
        public void settings_are_loaded_from_json_file()
        {
            var provider = new JsonSettingsProvider("settings.json");
            var settings = provider.Load();
            Assert.AreEqual("1", settings["sample1"]);
            Assert.AreEqual("2", settings["sample2"]);
        }

        [Test]
        public void an_exception_is_thrown_if_environment_not_found()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                {
                    var provider = new JsonSettingsProvider("broken.settings.json");
                    provider.Load();
                });
        }
    }
}
