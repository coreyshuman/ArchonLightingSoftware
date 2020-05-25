using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchonLightingSystem.Common
{
    class DataGridViewHandlers
    {
        public static DataGridViewCellValidatingEventHandler DataGridValidateNumericRangeHandler(Tuple<int, int> minMax)
        {
            return (object sender, DataGridViewCellValidatingEventArgs e) =>
            {
                if (ValidateCellNumericRange(sender, e, minMax.Item1, minMax.Item2))
                {
                    e.Cancel = true;
                }
            };
        }


        private static bool ValidateCellNumericRange(object sender, DataGridViewCellValidatingEventArgs e, int min, int max)
        {
            var gView = ((DataGridView)sender);
            string valStr = e.FormattedValue.ToString();
            int number = 0;
            // Confirm that the cell is not empty.
            if (string.IsNullOrEmpty(valStr))
            {
                gView.Rows[e.RowIndex].ErrorText = "Value can not be empty";
                return true;
            }

            if (!int.TryParse(valStr, out number))
            {
                gView.Rows[e.RowIndex].ErrorText = "Value must be a number";
                return true;
            }

            if (number < min || number > max)
            {
                gView.Rows[e.RowIndex].ErrorText = $"Value must be between {min} and {max}";
                return true;
            }

            gView.Rows[e.RowIndex].ErrorText = "";
            return false;
        }

    }
}
