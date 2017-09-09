using System;
using MMF.MME.VariableSubscriber.ConstantSubscriber.ConstantBufferManager;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.ConstantSubscriber
{
    internal class FullMaterialConstantSubscriber : ConstantBufferSubscriberBase, IDisposable
    {
        private readonly FullMaterialConstantBufferManager Manager;

        private FullMaterialConstantSubscriber(FullMaterialConstantBufferManager manager)
        {
            Manager = manager;
        }

        internal FullMaterialConstantSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "FULLMATERIALCONSTANT"; }
        }

        public override UpdateBy UpdateTiming
        {
            get { return UpdateBy.Material; }
        }

        public void Dispose()
        {
            if (Manager != null) Manager.Dispose();
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            FullMaterialConstantBufferInputLayout layout = new FullMaterialConstantBufferInputLayout
            {
                AmbientColor = variable.Material.AmbientColor,
                DiffuseColor = variable.Material.DiffuseColor,
                SpecularColor = variable.Material.SpecularColor,
                SpecularPower = variable.Material.SpecularPower,
                AddingSphereTexture = variable.Material.SphereAddValue,
                AddingTexture = variable.Material.TextureAddValue,
                EdgeColor = variable.Material.EdgeColor,
                EmissiveColor = variable.Material.EmissiveColor,
                GroundShadowColor = variable.Material.GroundShadowColor,
                MultiplyingSphereTexture = variable.Material.SphereMulValue,
                MultiplyingTexture = variable.Material.TextureMulValue,
                ToonColor = variable.Material.ToonColor
            };
            Manager.ApplyToEffect(layout);
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            FullMaterialConstantBufferManager manager = new FullMaterialConstantBufferManager();
            manager.Initialize(context.DeviceManager.Device, (EffectConstantBuffer) variable,
                FullMaterialConstantBufferInputLayout.SizeInBytes, new FullMaterialConstantBufferInputLayout());
            return new FullMaterialConstantSubscriber(manager);
        }
    }
}