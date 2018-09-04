using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ArchonLightingSystem.Components
{
    [Designer(typeof(VerticalProgressBarDesigner))] 
    public class VerticalProgressBar : ProgressBar
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x04;
                return cp;
            }
        }
    }

    class VerticalProgressBarDesigner : ControlDesigner
    {
        public override void Initialize(IComponent comp)
        {
            base.Initialize(comp);
            //var uc = (UserControl1)comp;
            //EnableDesignMode(uc.Employees, "Employees");
        }
    }
}
