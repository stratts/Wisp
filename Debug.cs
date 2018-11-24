using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wisp.Nodes;

namespace Wisp
{ 
    public static class Debug
    {
        public static string Output { get; private set; }

        public static void AddOutput(string output)
        {
            Output += output;
        }

        public static void ClearOutput()
        {
            Output = "";
        }
    }
}
