namespace MMF.Matricies.World
{
    public class WorldMatrixChangedEventArgs : System.EventArgs
    {
        public ChangedWorldMatrixValueType ChangedType
        {
            get;
            private set;
        }

        public WorldMatrixChangedEventArgs(ChangedWorldMatrixValueType type)
        {
            ChangedType = type;
        }
    }
}
