using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Common
{
    class Definitions
    {
        public const string BootloaderVid = "04D8";
        public const string BootloaderPid = "3033";
        public const string ApplicationVid = "04D8";
        public const string ApplicationPid = "3034";
        public const uint ApplicationVersionAddress = 0x9D00EFF0;
        public static readonly Version SoftwareVersion = new Version(1,3,20,0);
    }
}
