using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.OpenHardware
{
    internal class SensorUnits
    {
        public const int RangeCount = 11;

        public static string GetLabel(ISensor sensor)
        {
            string label;

            switch (sensor?.SensorType)
            {
                case SensorType.Voltage:
                    label = "V";
                    break;
                case SensorType.Clock:
                    label = "MHz";
                    break;
                case SensorType.Load:
                    label = "%";
                    break;
                case SensorType.Fan:
                    label = "RPM";
                    break;
                case SensorType.Flow:
                    label = "L/h";
                    break;
                case SensorType.Control:
                    label = "%";
                    break;
                case SensorType.Level:
                    label = "%";
                    break;
                case SensorType.Power:
                    label = "W";
                    break;
                case SensorType.Data:
                    label = "GB";
                    break;
                case SensorType.SmallData:
                    label = "MB";
                    break;
                case SensorType.Factor:
                    label = "";
                    break;
                case SensorType.Frequency:
                    label = "Hz";
                    break;
                case SensorType.Throughput:
                    label = "MB/s";
                    break;
                default: // fall-through
                case SensorType.Temperature:
                    label = "°C";
                    break;
            }

            return label;
        }

        public static double GetMin(ISensor sensor)
        {
            return 0;
        }

        public static double GetMax(ISensor sensor)
        {
            double maximum = 100;

            switch (sensor?.SensorType)
            {
                case SensorType.Voltage:
                    maximum = 13;
                    break;
                case SensorType.Clock:
                    maximum = 6000;
                    break;
                case SensorType.Load:
                    maximum = 100;
                    break;
                case SensorType.Fan:
                    maximum = 3000;
                    break;
                case SensorType.Flow:
                    maximum = 100;
                    break;
                case SensorType.Control:
                    maximum = 100;
                    break;
                case SensorType.Level:
                    maximum = 100;
                    break;
                case SensorType.Power:
                    maximum = 500;
                    break;
                case SensorType.Data:
                    maximum = 1000;
                    break;
                case SensorType.SmallData:
                    maximum = 1000;
                    break;
                case SensorType.Factor:
                    maximum = 100;
                    break;
                case SensorType.Frequency:
                    maximum = 1000;
                    break;
                case SensorType.Throughput:
                    maximum = 10000;
                    break;
                default: // fall-through
                case SensorType.Temperature:
                    maximum = 100;
                    break;
            }

            return maximum;
        }

        public static double GetInterval(ISensor sensor)
        {
            double min = GetMin(sensor);
            double max = GetMax(sensor);

            return (max - min) / (double)(RangeCount - 1);
        }

        public static double[] GetRange(ISensor sensor)
        {
            double min = GetMin(sensor);
            double max = GetMax(sensor);
            double step = GetInterval(sensor);

            double[] range = new double[RangeCount];

            for(int i = 0; i < range.Length - 1; i++)
            {
                range[i] = (min + step * i);
            }

            range[RangeCount - 1] = max;

            return range;
        }
    }
}
