namespace MMF.Matricies.Camera
{
    public class CameraMatrixChangedEventArgs : System.EventArgs
    {
        public CameraMatrixChangedVariableType ChangedType
        {
            get;
            private set;
        }

        public CameraMatrixChangedEventArgs(CameraMatrixChangedVariableType type)
        {
            ChangedType = type;
        }
    }
}
