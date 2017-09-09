namespace MMF.MME.Includer
{
    public class IncludeDirectory
    {
        public string DirectoryPath
        {
            get;
            private set;
        }

        public int Priorty
        {
            get;
            private set;
        }

        public IncludeDirectory(string directory, int priorty)
        {
            DirectoryPath = directory;
            Priorty = priorty;
        }
    }
}
