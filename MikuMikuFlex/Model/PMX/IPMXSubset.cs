namespace MMF.Model.PMX
{
    public interface IPMXSubset : ISubset, System.IDisposable
    {
        int StartIndex
        {
            get;
        }

        int VertexCount
        {
            get;
        }
    }
}
