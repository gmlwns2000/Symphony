using System;
using MMF.MME.VariableSubscriber.ConstantSubscriber.ConstantBufferManager;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.ConstantSubscriber
{
    public sealed class BasicMaterialConstantSubscriber : ConstantBufferSubscriberBase, IDisposable
    {
        private readonly BasicMaterialConstantBufferManager Manager;

        private BasicMaterialConstantSubscriber(BasicMaterialConstantBufferManager manager)
        {
            Manager = manager;
        }

        internal BasicMaterialConstantSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "BASICMATERIALCONSTANT"; }
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
            BasicMaterialConstantBufferInputLayout layout = new BasicMaterialConstantBufferInputLayout
            {
                AmbientLight = variable.Material.AmbientColor,
                DiffuseLight = variable.Material.DiffuseColor,
                SpecularLight = variable.Material.SpecularColor,
                SpecularPower = variable.Material.SpecularPower
            };
            Manager.ApplyToEffect(layout);
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            BasicMaterialConstantBufferManager manager = new BasicMaterialConstantBufferManager();
            manager.Initialize(context.DeviceManager.Device, (EffectConstantBuffer) variable,
                BasicMaterialConstantBufferInputLayout.SizeInBytes, new BasicMaterialConstantBufferInputLayout());
            return new BasicMaterialConstantSubscriber(manager);
        }
    }
}