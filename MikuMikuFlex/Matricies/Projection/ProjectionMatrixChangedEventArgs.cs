namespace MMF.Matricies.Projection
{
    public class ProjectionMatrixChangedEventArgs : System.EventArgs
    {
        public ProjectionMatrixChangedVariableType ChangedType
        {
            get;
            private set;
        }

        public ProjectionMatrixChangedEventArgs(ProjectionMatrixChangedVariableType type)
        {
            ChangedType = type;
        }
    }
}
