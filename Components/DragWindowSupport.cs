using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchonLightingSystem.Components
{
    class DragWindowSupport
    {
        private bool IsWindowMoving = false;
        private Point WindowOffset;
        private Form Form;

        public DragWindowSupport()
        {

        }

        public void Initialize(Form form, Control dragComponent = null)
        {
            Form = form;
            if(dragComponent != null)
            {
                dragComponent.MouseDown += MouseDownEventHandler;
                dragComponent.MouseMove += MouseMoveEventHandler;
                dragComponent.MouseUp += MouseUpEventHandler;
            } 
            else
            {
                form.MouseDown += MouseDownEventHandler;
                form.MouseMove += MouseMoveEventHandler;
                form.MouseUp += MouseUpEventHandler;
            }
        }

        private void MouseDownEventHandler(object sender, MouseEventArgs e)
        {
            IsWindowMoving = true;
            WindowOffset = new Point(e.X, e.Y);
        }

        private void MouseMoveEventHandler(object sender, MouseEventArgs e)
        {
            if (IsWindowMoving)
            {
                Point newlocation = Form.Location;
                newlocation.X += e.X - WindowOffset.X;
                newlocation.Y += e.Y - WindowOffset.Y;
                Form.Location = newlocation;
            }
        }
        private void MouseUpEventHandler(object sender, MouseEventArgs e)
        {
            if (IsWindowMoving)
            {
                IsWindowMoving = false;
            }
        }
    }
}
