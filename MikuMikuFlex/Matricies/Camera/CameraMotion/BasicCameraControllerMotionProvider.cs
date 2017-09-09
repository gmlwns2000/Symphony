using MMF.Matricies.Projection;
using SlimDX;
using System.Windows.Forms;

namespace MMF.Matricies.Camera.CameraMotion
{
    public class BasicCameraControllerMotionProvider : ICameraMotionProvider
    {
        private Vector3 xAxis = new Vector3(1f, 0f, 0f);

        private Vector3 yAxis = new Vector3(0f, 1f, 0f);

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

        private Quaternion cameraPositionRotation
        {
            get;
            set;
        }

        private Vector2 cameraLookatTranslation
        {
            get;
            set;
        }

        private float distance
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

        public BasicCameraControllerMotionProvider(Control control, Control wheelRevieveControl, float initialDistance = 45f)
        {
            distance = initialDistance;
            cameraPositionRotation = Quaternion.Identity;
            control.MouseDown += new MouseEventHandler(panel_MouseDown);
            control.MouseMove += new MouseEventHandler(panel_MouseMove);
            control.MouseUp += new MouseEventHandler(panel_MouseUp);
            wheelRevieveControl.MouseWheel += new MouseEventHandler(wheelRevieveControl_MouseWheel);
            MouseWheelSensibility = 2f;
            RightButtonRotationSensibility = 0.005f;
            MiddleButtonTranslationSensibility = 0.01f;
        }

        private void wheelRevieveControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
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

        private void panel_MouseMove(object sender, MouseEventArgs e)
        {
            int num = e.Location.X - LastMousePosition.X;
            int num2 = e.Location.Y - LastMousePosition.Y;
            if (isRightMousePushed)
            {
                cameraPositionRotation *= Quaternion.RotationAxis(yAxis, RightButtonRotationSensibility * num) * Quaternion.RotationAxis(xAxis, RightButtonRotationSensibility * -num2);
                cameraPositionRotation.Normalize();
            }
            if (isMiddleMousePushed)
            {
                cameraLookatTranslation += new Vector2(num, num2) * MiddleButtonTranslationSensibility;
            }
            LastMousePosition = e.Location;
        }

        private void panel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isRightMousePushed = false;
            }
            if (e.Button == MouseButtons.Middle)
            {
                isMiddleMousePushed = false;
            }
        }

        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isRightMousePushed = true;
            }
            if (e.Button == MouseButtons.Middle)
            {
                isMiddleMousePushed = true;
            }
        }

        void ICameraMotionProvider.UpdateCamera(CameraProvider cp1, IProjectionMatrixProvider proj)
        {
            Vector3 vector = Vector3.TransformCoordinate(new Vector3(0f, 0f, 1f), Matrix.RotationQuaternion(cameraPositionRotation));
            xAxis = Vector3.Cross(vector, cp1.CameraUpVec);
            xAxis.Normalize();
            yAxis = Vector3.Cross(xAxis, vector);
            yAxis.Normalize();
            cp1.CameraLookAt += xAxis * cameraLookatTranslation.X + yAxis * cameraLookatTranslation.Y;
            cp1.CameraPosition = cp1.CameraLookAt + distance * -vector;
            cameraLookatTranslation = Vector2.Zero;
        }
    }
}