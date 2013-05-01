using System.Collections.Generic;

namespace Trackmatic.Figs
{
    public interface IProvideSettings
    {
        Dictionary<string, string> Load();
    }
}
