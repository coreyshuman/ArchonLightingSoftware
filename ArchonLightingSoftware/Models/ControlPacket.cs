using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSystem.UsbApplication;

namespace ArchonLightingSystem.Models
{
    public class ControlPacket
    {
        public UsbApp.CONTROL_CMD Cmd;
        public UInt16 Len;
        public Byte[] Data;

        public ControlPacket()
        {
            Cmd = UsbApp.CONTROL_CMD.CMD_ERROR_OCCURED;
            Len = 0;
            Data = new byte[UsbApp.CONTROL_BUFFER_SIZE];
        }
    }
}
