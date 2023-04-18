using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchonLightingSystem.UsbApplicationV2;

namespace ArchonLightingSystem.Models
{
    public class FrameInfo
    {
        public Byte[] FrameData;
        public uint FrameLen;
        public bool Multiframe;
        public bool FrameValid;

        public FrameInfo()
        {
            FrameData = new byte[UsbDeviceManager.USB_PACKET_SIZE];
            Reset();
        }

        public void Reset()
        {
            FrameLen = 0;
            Multiframe = false;
            FrameValid = false;
        }
    }
}
