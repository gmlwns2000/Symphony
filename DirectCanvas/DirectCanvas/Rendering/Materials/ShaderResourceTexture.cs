using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;

namespace DirectCanvas.Rendering.Materials
{
    class ShaderResourceTexture : Texture
    {
        protected ShaderResourceTexture(Texture2D texture) : base(texture)
        {
            /* The shader resource view allows us to use this texture as a shader input */
            InternalShaderResourceView = new ShaderResourceView(InternalDevice, InternalTexture2D);
        }

        public ShaderResourceView InternalShaderResourceView { get; private set; }

        public override void Dispose()
        {
            InternalShaderResourceView.Dispose();

            base.Dispose();
        }
    }
}
