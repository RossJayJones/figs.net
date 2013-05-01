using System.Collections.Generic;
using System.IO;

namespace Trackmatic.Figs
{
    public interface IParseConfiguration
    {
        string Parse(Stream stream, Dictionary<string, string> settings);
    }
}
