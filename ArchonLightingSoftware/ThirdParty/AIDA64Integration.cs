using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.MemoryMappedFiles;
using System.Security.AccessControl;
using System.Runtime.InteropServices;

namespace ArchonLightingSystem.ThirdParty
{
    public static class AIDA64Integration
    {
        public static string ReadData()
        {
            MemoryMappedFile memoryMappedFile;
            string aidaReadings;

            memoryMappedFile = MemoryMappedFile.OpenExisting(
                "AIDA64_SensorValues",
                MemoryMappedFileRights.Read,
                System.IO.HandleInheritability.Inheritable
           );

            int size = 10 * 1024;

            using (var accessor = memoryMappedFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read))
            {
                
                byte[] data = new byte[size];

                // Read from memory mapped file.
                accessor.ReadArray<byte>(0, data, 0, data.Length);

                // convert to string
                aidaReadings = Encoding.UTF8.GetString(data, 0, data.Length);

                
            }

            memoryMappedFile.Dispose();
            return aidaReadings;
        }
    }
}
