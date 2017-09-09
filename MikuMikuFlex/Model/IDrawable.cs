using SlimDX;

namespace MMF.Model
{
    public interface IDrawable : System.IDisposable
    {
        bool Visibility
        {
            get;
            set;
        }

        string FileName
        {
            get;
        }

        int SubsetCount
        {
            get;
        }

        int VertexCount
        {
            get;
        }

        ITransformer Transformer
        {
            get;
        }

        Vector4 SelfShadowColor
        {
            get;
            set;
        }

        Vector4 GroundShadowColor
        {
            get;
            set;
        }

        void Draw();

        void Update();
    }
}
