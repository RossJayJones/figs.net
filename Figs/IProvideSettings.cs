using System.Collections.Generic;

namespace Figs
{
    public interface IProvideSettings
    {
        Dictionary<string, string> Load();
    }
}
