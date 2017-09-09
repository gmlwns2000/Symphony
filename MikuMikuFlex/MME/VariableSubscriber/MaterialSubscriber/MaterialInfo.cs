using Assimp;
using MMDFileParser.PMXModelParser;
using MMF.Model;
using MMF.Model.Assimp;
using MMF.Utility;
using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    public class MaterialInfo : System.IDisposable
    {
        public Vector4 AmbientColor;

        public Vector4 DiffuseColor;

        public Vector4 EdgeColor;

        public Vector4 EmissiveColor;

        public Vector4 GroundShadowColor;

        public bool IsOperationAdd;

        public bool IsSubtextureUsed;

        public bool IsToonUsed;

        public ShaderResourceView MaterialSphereMap;

        public ShaderResourceView MaterialTexture;

        public Vector4 SpecularColor;

        public float SpecularPower;

        public Vector4 SphereAddValue;

        public SphereMode SphereMode;

        public Vector4 SphereMulValue;

        public Vector4 TextureAddValue;

        public Vector4 TextureMulValue;

        public Vector4 ToonColor;

        public ShaderResourceView ToonTexture;

        public float EdgeSize;

        public bool isEdgeEnable;

        public bool isGroundShadowEnable;

        public MaterialInfo MulMaterialInfo;

        public MaterialInfo AddMaterialInfo;

        public MaterialInfo InitialMaterialInfo;

        public void Dispose()
        {
            if (MulMaterialInfo != null)
            {
                MulMaterialInfo.Dispose();
            }
            if (AddMaterialInfo != null)
            {
                AddMaterialInfo.Dispose();
            }
            if (MaterialTexture != null && !MaterialTexture.Disposed)
            {
                MaterialTexture.Dispose();
            }
            if (MaterialSphereMap != null && !MaterialSphereMap.Disposed)
            {
                MaterialSphereMap.Dispose();
            }
            if (ToonTexture != null && !ToonTexture.Disposed)
            {
                ToonTexture.Dispose();
            }
        }

        public void UpdateMaterials()
        {
            AmbientColor = CGHelper.MulEachMember(InitialMaterialInfo.AmbientColor, MulMaterialInfo.AmbientColor) + AddMaterialInfo.AmbientColor;
            DiffuseColor = CGHelper.MulEachMember(InitialMaterialInfo.DiffuseColor, MulMaterialInfo.DiffuseColor) + AddMaterialInfo.DiffuseColor;
            SpecularColor = CGHelper.MulEachMember(InitialMaterialInfo.SpecularColor, MulMaterialInfo.SpecularColor) + AddMaterialInfo.SpecularColor;
            SpecularPower = InitialMaterialInfo.SpecularPower * MulMaterialInfo.SpecularPower + AddMaterialInfo.SpecularPower;
            EdgeColor = CGHelper.MulEachMember(InitialMaterialInfo.EdgeColor, MulMaterialInfo.EdgeColor) + AddMaterialInfo.EdgeColor;
            ResetMorphMember();
        }

        private void ResetMorphMember()
        {
            MulMaterialInfo.AmbientColor = new Vector4(1f);
            MulMaterialInfo.DiffuseColor = new Vector4(1f);
            MulMaterialInfo.SpecularColor = new Vector4(1f);
            MulMaterialInfo.SpecularPower = 1f;
            MulMaterialInfo.EdgeColor = new Vector4(0f);
            AddMaterialInfo.AmbientColor = new Vector4(0f);
            AddMaterialInfo.DiffuseColor = new Vector4(0f);
            AddMaterialInfo.SpecularColor = new Vector4(0f);
            AddMaterialInfo.SpecularPower = 0f;
            AddMaterialInfo.EdgeColor = new Vector4(0f);
        }

        public static MaterialInfo FromMaterialData(IDrawable drawable, MaterialData data)
        {
            MaterialInfo materialInfo = new MaterialInfo();
            materialInfo.AmbientColor = new Vector4(data.Ambient, 1f);
            materialInfo.DiffuseColor = data.Diffuse;
            materialInfo.EdgeColor = data.EdgeColor;
            materialInfo.EmissiveColor = new Vector4(1f);
            materialInfo.GroundShadowColor = drawable.GroundShadowColor;
            materialInfo.SpecularColor = new Vector4(data.Specular, 1f);
            materialInfo.SpecularPower = data.SpecularCoefficient;
            materialInfo.ToonColor = new Vector4(0f);
            materialInfo.EdgeSize = data.EdgeSize;
            materialInfo.isEdgeEnable = data.bitFlag.HasFlag(RenderFlag.RenderEdge);
            materialInfo.isGroundShadowEnable = data.bitFlag.HasFlag(RenderFlag.GroundShadow);
            materialInfo.MulMaterialInfo = new MaterialInfo();
            materialInfo.AddMaterialInfo = new MaterialInfo();
            materialInfo.InitialMaterialInfo = new MaterialInfo
            {
                AmbientColor = new Vector4(data.Ambient, 1f),
                DiffuseColor = data.Diffuse,
                EdgeColor = data.EdgeColor,
                EmissiveColor = new Vector4(1f),
                GroundShadowColor = drawable.GroundShadowColor,
                SpecularColor = new Vector4(data.Specular, 1f),
                SpecularPower = data.SpecularCoefficient,
                ToonColor = new Vector4(0f),
                EdgeSize = data.EdgeSize
            };
            materialInfo.ResetMorphMember();
            return materialInfo;
        }

        public static MaterialInfo FromMaterialData(IDrawable drawable, Material material, RenderContext context, ISubresourceLoader loader)
        {
            MaterialInfo materialInfo = new MaterialInfo();
            materialInfo.AmbientColor = material.ColorAmbient.ToSlimDX();
            if (materialInfo.AmbientColor == Vector4.Zero)
            {
                materialInfo.AmbientColor = new Vector4(1f, 1f, 1f, 1f);
            }
            materialInfo.DiffuseColor = material.ColorDiffuse.ToSlimDX();
            if (materialInfo.DiffuseColor == Vector4.Zero)
            {
                materialInfo.DiffuseColor = new Vector4(1f, 1f, 1f, 1f);
            }
            else if (materialInfo.DiffuseColor.W == 0f)
            {
                materialInfo.DiffuseColor.W = 1f;
            }
            materialInfo.EmissiveColor = material.ColorEmissive.ToSlimDX();
            materialInfo.SpecularColor = material.ColorSpecular.ToSlimDX();
            materialInfo.SpecularPower = material.ShininessStrength;
            if (materialInfo.SpecularColor == Vector4.Zero)
            {
                materialInfo.SpecularColor = new Vector4(0.1f);
            }
            materialInfo.GroundShadowColor = drawable.GroundShadowColor;
            materialInfo.isEdgeEnable = false;
            materialInfo.isGroundShadowEnable = true;
            materialInfo.SphereMode = SphereMode.Disable;
            materialInfo.IsToonUsed = false;
            if (material.GetTextures(TextureType.Diffuse) != null)
            {
                System.IO.Stream subresourceByName = loader.getSubresourceByName(material.GetTextures(TextureType.Diffuse)[0].FilePath);
                if (subresourceByName != null)
                {
                    using (Texture2D texture2D = Texture2D.FromStream(context.DeviceManager.Device, subresourceByName, (int)subresourceByName.Length))
                    {
                        materialInfo.MaterialTexture = new ShaderResourceView(context.DeviceManager.Device, texture2D);
                    }
                }
                subresourceByName.Close();
            }
            return materialInfo;
        }
    }
}
