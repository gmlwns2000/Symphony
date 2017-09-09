using MMDFileParser.PMXModelParser;
using MMF.MME;
using SlimDX.Direct3D11;

namespace MMF.Model.PMX
{
    internal class PMXSubsetManager : ISubsetManager, System.IDisposable
    {
        private readonly ModelData model;

        private Device device;

        private ISubresourceLoader subresourceManager;

        private IToonTextureManager toonManager;

        public System.Collections.Generic.List<ISubset> Subsets
        {
            get;
            private set;
        }

        public int SubsetCount
        {
            get
            {
                return Subsets.Count;
            }
        }

        public IDrawable Drawable
        {
            get;
            set;
        }

        public MMEEffectManager MMEEffect
        {
            get;
            set;
        }

        public PMXSubsetManager(PMXModel drawable, ModelData model)
        {
            this.model = model;
            Drawable = drawable;
        }

        public void Initialze(RenderContext context, MMEEffectManager effectManager, ISubresourceLoader subresourceManager, IToonTextureManager ToonManager)
        {
            MMEEffect = effectManager;
            toonManager = ToonManager;
            Subsets = new System.Collections.Generic.List<ISubset>();
            device = context.DeviceManager.Device;
            this.subresourceManager = subresourceManager;
            ModelData modelData = model;
            int num = 0;
            for (int i = 0; i < modelData.MaterialList.MaterialCount; i++)
            {
                MaterialData materialData = modelData.MaterialList.Materials[i];
                PMXSubset pMXSubset = new PMXSubset(Drawable, materialData, i);
                pMXSubset.DoCulling = !materialData.bitFlag.HasFlag(RenderFlag.CullNone);
                pMXSubset.VertexCount = materialData.VertexNumber / 3;
                pMXSubset.StartIndex = num;
                if (materialData.textureIndex >= toonManager.ResourceViews.Length)
                {
                    if (toonManager.ResourceViews.Length == 0)
                    {
                        int num2 = ToonManager.LoadToon(modelData.TextureList.TexturePathes[materialData.textureIndex]);
                        pMXSubset.MaterialInfo.ToonTexture = ToonManager.ResourceViews[num2];
                        pMXSubset.MaterialInfo.IsToonUsed = false;
                    }
                    else
                    {
                        pMXSubset.MaterialInfo.ToonTexture = ToonManager.ResourceViews[0];
                        pMXSubset.MaterialInfo.IsToonUsed = false;
                    }
                }
                else if (materialData.ShareToonFlag == 1)
                {
                    pMXSubset.MaterialInfo.ToonTexture = ToonManager.ResourceViews[materialData.textureIndex + 1];
                    pMXSubset.MaterialInfo.IsToonUsed = true;
                }
                else if (materialData.textureIndex != -1)
                {
                    if (modelData.TextureList.TexturePathes.Count < materialData.textureIndex + 1)
                    {
                        pMXSubset.MaterialInfo.ToonTexture = ToonManager.ResourceViews[0];
                        pMXSubset.MaterialInfo.IsToonUsed = true;
                    }
                    else
                    {
                        int num2 = ToonManager.LoadToon(modelData.TextureList.TexturePathes[materialData.textureIndex]);
                        pMXSubset.MaterialInfo.ToonTexture = ToonManager.ResourceViews[num2];
                        pMXSubset.MaterialInfo.IsToonUsed = true;
                    }
                }
                else
                {
                    pMXSubset.MaterialInfo.ToonTexture = ToonManager.ResourceViews[0];
                    pMXSubset.MaterialInfo.IsToonUsed = true;
                }
                num += materialData.VertexNumber;
                if (materialData.TextureTableReferenceIndex != -1)
                {
                    pMXSubset.MaterialInfo.MaterialTexture = getSubresourceById(modelData.TextureList.TexturePathes[materialData.TextureTableReferenceIndex]);
                }
                if (materialData.SphereTextureTableReferenceIndex != -1)
                {
                    pMXSubset.MaterialInfo.SphereMode = materialData.SphereMode;
                    pMXSubset.MaterialInfo.MaterialSphereMap = getSubresourceById(modelData.TextureList.TexturePathes[materialData.SphereTextureTableReferenceIndex]);
                }
                Subsets.Add(pMXSubset);
            }
        }

        public void ResetEffect(MMEEffectManager effect)
        {
            MMEEffect = effect;
        }

        public void DrawAll()
        {
            for (int i = 0; i < Subsets.Count; i++)
            {
                UpdateConstantByMaterial(Subsets[i]);
                MMEEffect.ApplyEffectPass(Subsets[i], MMEEffectPassType.Object, delegate (ISubset subset)
                {
                    subset.Draw(device);
                });
            }
        }

        public void DrawEdges()
        {
        }

        public void DrawGroundShadow()
        {
        }

        public void Dispose()
        {
            using (System.Collections.Generic.List<ISubset>.Enumerator enumerator = Subsets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    PMXSubset pMXSubset = (PMXSubset)enumerator.Current;
                    pMXSubset.Dispose();
                }
            }
        }

        private void UpdateConstantByMaterial(ISubset ipmxSubset)
        {
            MMEEffect.ApplyAllMaterialVariables(ipmxSubset.MaterialInfo);
        }

        private ShaderResourceView getSubresourceById(string p)
        {
            ShaderResourceView result;
            using (System.IO.Stream subresourceByName = subresourceManager.getSubresourceByName(p))
            {
                if (subresourceByName == null)
                {
                    result = null;
                }
                else
                {
                    result = ShaderResourceView.FromStream(device, subresourceByName, (int)subresourceByName.Length);
                }
            }
            return result;
        }
    }
}