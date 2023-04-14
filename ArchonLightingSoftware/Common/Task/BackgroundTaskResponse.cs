using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Common
{
    /// <summary>
    /// Required response value from an IBackgroundTask function.
    /// Continue will keep the task loop going.
    /// Exit will end the background task.
    /// </summary>
    public enum BackgroundTaskResponse
    {
        Continue = 0,
        Exit
    }
}
