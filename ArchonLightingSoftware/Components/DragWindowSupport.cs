using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArchonLightingSystem.Components
{
    public class DragWindowEventArgs
    {
        public Point Location { get; }

        public DragWindowEventArgs(Point loc)
        {
            Location = loc;
        }
    }

    public delegate void DragWindowEventDelegate(object sender, DragWindowEventArgs args);

    class DragWindowSupport
    {
        private bool IsWindowMoving = false;
        private Point WindowOffset;
        private Form Form;

        public event DragWindowEventDelegate DragWindowEvent;

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
                OnDragEvent(Form.Location.X, Form.Location.Y);
            }
        }

        private void OnDragEvent(int x, int y)
        {
            DragWindowEvent?.Invoke(this, new DragWindowEventArgs(Form.Location));
        }
    }
}
