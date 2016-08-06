using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Construction;

namespace Trackmatic.Figs.Scratch.Pad
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(@"C:\Data\Projects\trackmatic", @"**\bin\Debug\*.config", SearchOption.AllDirectories);
            

            var file = Microsoft.Build.Construction.
        }
    }
}
