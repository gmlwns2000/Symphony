namespace MMF.MME
{
    public class EffectLoaderCompilingEventArgs : System.EventArgs
    {
        private string targetFileName;

        public string TargetFileName
        {
            get
            {
                return targetFileName;
            }
        }

        public EffectLoaderCompilingEventArgs(string targetFileName)
        {
            this.targetFileName = targetFileName;
        }
    }
}
