using SlimDX.Direct3D11;
using System.Collections.Generic;

namespace MMF.Model.PMX
{
    internal class PMXToonTextureManager : IToonTextureManager, System.IDisposable
    {
        private readonly List<ShaderResourceView> resourceViewsList = new System.Collections.Generic.List<ShaderResourceView>();

        private Device _device;

        private ISubresourceLoader _subresourceManager;

        public ShaderResourceView[] ResourceViews
        {
            get
            {
                return resourceViewsList.ToArray();
            }
        }

        public void Initialize(RenderContext context, ISubresourceLoader subresourceManager)
        {
            _device = context.DeviceManager.Device;
            _subresourceManager = subresourceManager;
            for (int i = 0; i < 11; i++)
            {
                string text = "Resource\\Toon\\" + string.Format("toon{0}.bmp", i);
                if (System.IO.File.Exists(text))
                {
                    resourceViewsList.Add(ShaderResourceView.FromFile(context.DeviceManager.Device, text));
                }
            }
        }

        public int LoadToon(string path)
        {
            int result;
            using (System.IO.Stream subresourceByName = _subresourceManager.getSubresourceByName(path))
            {
                if (subresourceByName == null)
                {
                    result = 0;
                }
                else
                {
                    resourceViewsList.Add(ShaderResourceView.FromStream(_device, subresourceByName, (int)subresourceByName.Length));
                    result = resourceViewsList.Count - 1;
                }
            }
            return result;
        }

        public void Dispose()
        {
            foreach (ShaderResourceView current in resourceViewsList)
            {
                current.Dispose();
            }
        }
    }
}