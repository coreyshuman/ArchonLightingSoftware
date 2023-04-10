using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;

namespace ArchonLightingSystem.Models
{
    public static class AppTheme
    {
        public static Color Background = Color.FromArgb(32, 32, 32);
        public static Color ComponentBackground = Color.FromArgb(64, 64, 64);
        public static Color PrimaryHighlight = Color.FromArgb(53, 114, 102);
        public static Color SecondaryHighlight = Color.FromArgb(204, 164, 59);
        public static Color PrimaryLowLight = Color.FromArgb(163, 11, 55);
        public static Color SecondaryLowLight = Color.FromArgb(244, 91, 105);
        public static Color PrimaryText = Color.FromArgb(232, 233, 243);

        public static Font ComponentFont = new Font(FontFamily.GenericSansSerif, 9.75f, FontStyle.Regular);
        public static Font ComponentFontLarge = new Font(FontFamily.GenericSansSerif, 12f, FontStyle.Bold);
        public static Font FormFont = new Font(FontFamily.GenericSansSerif, 9.75f, FontStyle.Bold);

        public static void ApplyThemeToForm(Form form)
        {
            form.BackColor = AppTheme.Background;
            form.ForeColor = AppTheme.PrimaryText;

            ApplyThemeToControls(form.Controls);
        }

        public static void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (var control in controls)
            {
                if (control is Label label)
                {
                    label.ForeColor = AppTheme.PrimaryText;
                    label.Font = AppTheme.FormFont;
                }

                if (control is Button button)
                {
                    button.ForeColor = AppTheme.PrimaryText;
                    button.BackColor = AppTheme.ComponentBackground;
                    button.Font = AppTheme.ComponentFont;
                }

                if (control is ComboBox combo)
                {
                    combo.ForeColor = AppTheme.PrimaryText;
                    combo.BackColor = AppTheme.ComponentBackground;
                    combo.Font = AppTheme.ComponentFont;
                    
                }

                if (control is DataGridView grid)
                {
                    grid.ForeColor = AppTheme.PrimaryText;
                    grid.BackColor = AppTheme.Background;
                    grid.Font = AppTheme.ComponentFont;
                    grid.GridColor = AppTheme.PrimaryText;
                    grid.BackgroundColor = AppTheme.Background;
                    grid.BorderStyle = BorderStyle.None;

                    grid.ColumnHeadersDefaultCellStyle.Font = AppTheme.FormFont;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.ComponentBackground;
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.PrimaryText;
                    grid.RowHeadersDefaultCellStyle.Font = AppTheme.FormFont;
                    grid.RowHeadersDefaultCellStyle.BackColor = AppTheme.ComponentBackground;
                    grid.RowHeadersDefaultCellStyle.ForeColor = AppTheme.PrimaryText;
                    grid.EnableHeadersVisualStyles = true;
                    grid.DefaultCellStyle.BackColor = AppTheme.Background;
                    grid.DefaultCellStyle.ForeColor = AppTheme.PrimaryText;
                    grid.DefaultCellStyle.SelectionBackColor = AppTheme.PrimaryLowLight;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.ComponentBackground;
                    grid.AlternatingRowsDefaultCellStyle.ForeColor = AppTheme.PrimaryText;
                }

                if(control is BindingNavigator nav)
                {
                    nav.BackColor = AppTheme.ComponentBackground;
                    nav.ForeColor = AppTheme.PrimaryText;
                    
                }

                if (control is ContainerControl cControl)
                {
                    ApplyThemeToControls(cControl.Controls);
                }

                if (control is Panel panel)
                {
                    ApplyThemeToControls(panel.Controls);
                }
            }
        }
    }
}
