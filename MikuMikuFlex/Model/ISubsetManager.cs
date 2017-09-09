using MMF.MME;

namespace MMF.Model
{
    public interface ISubsetManager : System.IDisposable
    {
        int SubsetCount
        {
            get;
        }

        System.Collections.Generic.List<ISubset> Subsets
        {
            get;
        }

        void Initialze(RenderContext context, MMEEffectManager effect, ISubresourceLoader subresourceManager, IToonTextureManager ToonManager);

        void ResetEffect(MMEEffectManager effect);

        void DrawAll();

        void DrawEdges();

        void DrawGroundShadow();
    }
}
