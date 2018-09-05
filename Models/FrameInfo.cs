using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSystem.UsbApplication;

namespace ArchonLightingSystem.Models
{
    public class FrameInfo
    {
        public Byte[] FrameData;
        public uint FrameLen;
        public uint OutBufferMaxLen;
        public bool Multiframe;
        public bool FrameValid;

        public FrameInfo()
        {
            FrameData = new byte[UsbApp.USB_PACKET_SIZE];
            FrameLen = 0;
            OutBufferMaxLen = 512;
            Multiframe = false;
            FrameValid = false;
        }
    }
}
