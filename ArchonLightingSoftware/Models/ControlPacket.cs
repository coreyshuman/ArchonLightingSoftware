using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSystem.Common;

namespace ArchonLightingSystem.Models
{
    public class ControlPacket
    {
        public UsbAppCommon.CONTROL_CMD Cmd;
        public UInt16 Len;
        public Byte[] Data;

        public ControlPacket()
        {
            Data = new byte[UsbAppCommon.CONTROL_BUFFER_SIZE];
            Reset();
        }

        public void Reset()
        {
            Cmd = UsbAppCommon.CONTROL_CMD.CMD_ERROR_OCCURRED;
            Len = 0;
        }
    }
}
