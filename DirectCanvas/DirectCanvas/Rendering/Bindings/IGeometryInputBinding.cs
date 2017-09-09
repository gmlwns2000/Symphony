using System;

namespace DirectCanvas.Rendering.Bindings
{
    interface IGeometryInputBinding : IDisposable
    {
        void SetRenderState();
    }
}