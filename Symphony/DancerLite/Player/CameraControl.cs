using MMF.Matricies.Camera;
using MMF.Matricies.Projection;
using SlimDX;
using System.Windows.Controls;
using System.Windows.Input;

namespace Symphony.DancerLite
{
    class CameraControl : ICameraMotionProvider
    {
        public bool isLocked;

        private Vector3 xAxis = new Vector3(1f, 0f, 0f);

        private Vector3 yAxis = new Vector3(0f, 1f, 0f);

        private Control control;

        private System.Drawing.Point LastMousePosition
        {
            get;
            set;
        }

        private bool isRightMousePushed
        {
            get;
            set;
        }

        private bool isMiddleMousePushed
        {
            get;
            set;
        }

        protected Quaternion cameraPositionRotation
        {
            get;
            set;
        }

        protected Vector2 cameraLookatTranslation
        {
            get;
            set;
        }

        protected Vector3 cameraLookatTranslationOfWorld
        {
            get;
            set;
        }

        protected float distance
        {
            get;
            set;
        }

        public float MouseWheelSensibility
        {
            get;
            set;
        }

        public float RightButtonRotationSensibility
        {
            get;
            set;
        }

        public float MiddleButtonTranslationSensibility
        {
            get;
            set;
        }

        public CameraControl(Control control, float initialDistance = 45f)
        {
            distance = initialDistance;
            cameraPositionRotation = Quaternion.Identity;
            this.control = control;

            control.MouseDown += new MouseButtonEventHandler(panel_MouseDown);
            control.MouseMove += new MouseEventHandler(panel_MouseMove);
            control.MouseUp += new MouseButtonEventHandler(panel_MouseUp);
            control.MouseWheel += new MouseWheelEventHandler(wheelRevieveControl_MouseWheel);

            MouseWheelSensibility = 2f;
            RightButtonRotationSensibility = 6f;
            MiddleButtonTranslationSensibility = 5f;
        }

        private void wheelRevieveControl_MouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            if (!isLocked)
            {
                if (mouseWheelEventArgs.Delta > 0)
                {
                    distance -= MouseWheelSensibility;
                    if (distance <= 0f)
                    {
                        distance = 0.0001f;
                    }
                }
                else
                {
                    distance += MouseWheelSensibility;
                }
            }
        }

        private void panel_MouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (!isLocked)
            {
                int num = (int)(mouseEventArgs.GetPosition(control).X - LastMousePosition.X);
                int num2 = (int)(mouseEventArgs.GetPosition(control).Y - LastMousePosition.Y);
                if (isRightMousePushed)
                {
                    cameraPositionRotation *= Quaternion.RotationAxis(yAxis, RightButtonRotationSensibility / 1000f * num) * Quaternion.RotationAxis(xAxis, RightButtonRotationSensibility / 1000f * -num2);
                    cameraPositionRotation.Normalize();
                }
                if (isMiddleMousePushed)
                {
                    cameraLookatTranslation += new Vector2(num, num2) * MiddleButtonTranslationSensibility / 100f;
                }
                LastMousePosition = new System.Drawing.Point((int)mouseEventArgs.GetPosition(control).X, (int)mouseEventArgs.GetPosition(control).Y);
            }
        }

        private void panel_MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.RightButton == MouseButtonState.Released)
            {
                isRightMousePushed = false;
            }
            if (mouseButtonEventArgs.MiddleButton == MouseButtonState.Released)
            {
                isMiddleMousePushed = false;
            }
        }

        private void panel_MouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.RightButton == MouseButtonState.Pressed || mouseButtonEventArgs.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                isRightMousePushed = true;
            }

            if (mouseButtonEventArgs.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers == ModifierKeys.Control || mouseButtonEventArgs.MiddleButton == MouseButtonState.Pressed)
            {
                isMiddleMousePushed = true;
            }
        }

        public virtual void UpdateCamera(CameraProvider cp1, IProjectionMatrixProvider proj)
        {
            Vector3 vector = Vector3.TransformCoordinate(new Vector3(0f, 0f, 1f), Matrix.RotationQuaternion(cameraPositionRotation));
            xAxis = Vector3.Cross(vector, cp1.CameraUpVec);
            xAxis.Normalize();
            yAxis = Vector3.Cross(xAxis, vector);
            yAxis.Normalize();
            cp1.CameraLookAt += xAxis * cameraLookatTranslation.X + yAxis * cameraLookatTranslation.Y;
            cp1.CameraLookAt += cameraLookatTranslationOfWorld;
            cp1.CameraPosition = cp1.CameraLookAt + distance * -vector;
            cameraLookatTranslation = Vector2.Zero;
            cameraLookatTranslationOfWorld = Vector3.Zero;
        }
    }
}