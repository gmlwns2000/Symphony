using SlimDX.Direct3D11;

namespace MMF.Model
{
    public interface IBufferManager : System.IDisposable
    {
        Buffer VertexBuffer
        {
            get;
        }

        Buffer IndexBuffer
        {
            get;
        }

        BasicInputLayout[] InputVerticies
        {
            get;
        }

        bool NeedReset
        {
            get;
            set;
        }

        InputLayout VertexLayout
        {
            get;
        }

        int VerticiesCount
        {
            get;
        }

        void RecreateVerticies();

        void Initialize(object model, Device device, Effect effect);
    }
}
