using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.Udon;

namespace FC.Utils
{
    internal static class UdonUtils
    {
        internal static int GetScriptEventsCount(this UdonBehaviour instance)
        {
            
            return instance._eventTable.count;
        }
    }
}
