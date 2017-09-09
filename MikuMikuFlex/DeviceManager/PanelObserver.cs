using MMF.Motion;
using SlimDX;
using System.Windows.Forms;

namespace MMF.DeviceManager
{
    public class PanelObserver
    {
        public Vector2 MousePosition
        {
            get;
            private set;
        }

        public Vector4 LeftMouseDown
        {
            get;
            private set;
        }

        public Vector4 MiddleMouseDown
        {
            get;
            private set;
        }

        public Vector4 RightMouseDown
        {
            get;
            private set;
        }

        private Control RelatedControl
        {
            get;
            set;
        }

        public bool IsMMEMouseEnable
        {
            get;
            set;
        }

        public PanelObserver(Control control)
        {
            RelatedControl = control;
            IsMMEMouseEnable = false;
            control.MouseMove += new MouseEventHandler(MouseHandler);
            control.MouseDown += new MouseEventHandler(MouseHandler);
        }

        private void MouseHandler(object sender, MouseEventArgs e)
        {
            if (IsMMEMouseEnable)
            {
                float x = 0f;
                float y = 0f;
                float w = LeftMouseDown.W;
                float w2 = MiddleMouseDown.W;
                float w3 = RightMouseDown.W;
                float z = 0f;
                float z2 = 0f;
                float z3 = 0f;
                if (e.X != 0)
                {
                    x = e.X * 2f / RelatedControl.Width - 1f;
                }
                if (e.Y != 0)
                {
                    y = e.Y * 2f / RelatedControl.Height - 1f;
                }
                MousePosition = new Vector2(x, y);
                if (e.Button.HasFlag(MouseButtons.Left))
                {
                    w = MotionTimer.stopWatch.ElapsedMilliseconds / 1000f;
                    z = 1f;
                }
                if (e.Button.HasFlag(MouseButtons.Middle))
                {
                    w2 = MotionTimer.stopWatch.ElapsedMilliseconds / 1000f;
                    z2 = 1f;
                }
                if (e.Button.HasFlag(MouseButtons.Right))
                {
                    w3 = MotionTimer.stopWatch.ElapsedMilliseconds / 1000f;
                    z3 = 1f;
                }
                LeftMouseDown = new Vector4(x, y, z, w);
                MiddleMouseDown = new Vector4(x, y, z2, w2);
                RightMouseDown = new Vector4(x, y, z3, w3);
            }
        }
    }
}