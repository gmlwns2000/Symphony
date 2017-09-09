using System;
using MMF.MME.VariableSubscriber.MaterialSubscriber;
using SlimDX.Direct3D11;

namespace MMF.Model
{
    public interface ISubset:IDisposable
    {
        MaterialInfo MaterialInfo { get; }
        int SubsetId { get; }
        IDrawable Drawable { get; set; }
        bool DoCulling { get; }
        void Draw(Device device);
    }
}