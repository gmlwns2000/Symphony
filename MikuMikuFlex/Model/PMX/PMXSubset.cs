using MMDFileParser.PMXModelParser;
using MMF.MME.VariableSubscriber.MaterialSubscriber;
using SlimDX.Direct3D11;

namespace MMF.Model.PMX
{
    public class PMXSubset : IPMXSubset, ISubset, System.IDisposable
    {
        public MaterialInfo MaterialInfo
        {
            get;
            private set;
        }

        public int SubsetId
        {
            get;
            private set;
        }

        public int StartIndex
        {
            get;
            set;
        }

        public int VertexCount
        {
            get;
            set;
        }

        public IDrawable Drawable
        {
            get;
            set;
        }

        public bool DoCulling
        {
            get;
            set;
        }

        public PMXSubset(IDrawable drawable, MaterialData data, int subsetId)
        {
            Drawable = drawable;
            MaterialInfo = MaterialInfo.FromMaterialData(Drawable, data);
            SubsetId = subsetId;
        }

        public void Dispose()
        {
            if (MaterialInfo != null)
            {
                MaterialInfo.Dispose();
            }
        }

        public void Draw(Device device)
        {
            device.ImmediateContext.DrawIndexed(3 * VertexCount, StartIndex, 0);
        }
    }
}
