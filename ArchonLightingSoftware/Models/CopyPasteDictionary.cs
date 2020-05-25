using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSystem.Common;

namespace ArchonLightingSystem.Models
{
    static class CopyPasteDictionary
    {
        private static Dictionary<string, object> dataStore = new Dictionary<string, object>();

        public static void Copy(string name, object data)
        {
            data = Util.DeepCopy(data);
            if (dataStore.ContainsKey(name))
            {
                dataStore[name] = data;
            }
            else
            {
                dataStore.Add(name, data);
            }
        }

        public static object Paste(string name)
        {
            if(dataStore.ContainsKey(name))
            {
                return dataStore[name];
            }
            else
            {
                return null;
            }
        }
    }
}
