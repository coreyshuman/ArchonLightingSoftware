using ArchonLightingSystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchonLightingSystem.Models
{
    public class CalculationMethods
    {
        public static readonly string[] MethodNames = new string[] { "Max", "Min", "Average", "Sum" };

        public enum Methods
        {
            Max = 0,
            Min = 1,
            Average = 2,
            Sum = 3,
        }

        public static string GetName(Methods method)
        {
            return MethodNames[(int)method];
        }

        public static ComboBoxItem[] GetComboBoxItems()
        {
            return MethodNames.Select((method, index) => new ComboBoxItem { Text = method, Value= index }).ToArray();
        }
    }
}
