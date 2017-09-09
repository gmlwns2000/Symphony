using System;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D10.Device;

namespace DirectCanvas.Rendering.Materials
{
    /// <summary>
    /// Internal wrapper for a Direct3D Texture2D.  This
    /// class also keeps track of related resources to the Texture2D
    /// </summary>
    class Texture : IDisposable
    {
        public Texture(Texture2D texture)
        {
            InternalTexture2D = texture;

            /* Cache the description for perf reasons */
            Description = texture.Description;

            InternalDevice = InternalTexture2D.Device;

            /* SlimDX will only let us pull out the surface once until it is disposed.
             * We will do that here as it is the logic place to store ref to the DXGI surface */
        }

        /// <summary>
        /// The DXGI Surface that backs this Texture2D
        /// </summary>
        public Surface InternalDxgiSurface
        {
            get
            {
                if (_internalDxgiSurface == null)
                {
                    if (InternalTexture2D == null)
                        return null;

                    _internalDxgiSurface = InternalTexture2D.AsSurface();
                }

                return _internalDxgiSurface;
            }
        }
        private Surface _internalDxgiSurface;

        /// <summary>
        /// The Direct3D Texture2D instance
        /// </summary>
        public Texture2D InternalTexture2D { get; private set; }
        
        protected Device InternalDevice { get; private set; }

        /// <summary>
        /// The cached description of the Texture2D
        /// </summary>
        public Texture2DDescription Description { get; private set; }

        public virtual void Dispose()
        {
            if (InternalDxgiSurface != null)
            {
                InternalDxgiSurface.Dispose();
                _internalDxgiSurface = null;
            }

            if (InternalTexture2D != null)
            {
                InternalTexture2D.Dispose();
            }
        }
    }
}
