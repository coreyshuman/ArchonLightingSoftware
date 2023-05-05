using ArchonLightingSystem.OpenHardware;
using ArchonLightingSystem.UsbApplicationV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Interfaces
{
    public interface IService
    {
        public int TaskPeriod { get; set; }
        public void Pause(bool pause);
        public void BeginService(UsbControllerManager cm, SensorMonitorManager hw);
        public void CloseService();
    }
}
