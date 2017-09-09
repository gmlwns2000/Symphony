using MMF.Model;
using SlimDX.Direct3D11;

namespace MMF.MME.Script.Function
{
    internal class RenderDepthStencilTargetFunction : FunctionBase
    {
        private DepthStencilView stencilView;

        private RenderContext context;

        private bool isDefaultTarget;

        public override string FunctionName
        {
            get
            {
                return "RenderDepthStencilTarget";
            }
        }

        public override FunctionBase GetExecuterInstance(int index, string value, RenderContext context, ScriptRuntime runtime, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            RenderDepthStencilTargetFunction renderDepthStencilTargetFunction = new RenderDepthStencilTargetFunction();
            if (index != 0)
            {
                throw new InvalidMMEEffectShaderException("RenderDepthStencilTargetにはインデックス値を指定できません。");
            }
            renderDepthStencilTargetFunction.context = context;
            if (!string.IsNullOrWhiteSpace(value) && !manager.RenderDepthStencilTargets.ContainsKey(value))
            {
                throw new InvalidMMEEffectShaderException("スクリプトに指定された名前の深度ステンシルバッファは存在しません。");
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                renderDepthStencilTargetFunction.isDefaultTarget = true;
            }
            renderDepthStencilTargetFunction.stencilView = (string.IsNullOrWhiteSpace(value) ? null : manager.RenderDepthStencilTargets[value]);
            return renderDepthStencilTargetFunction;
        }

        public override void Execute(ISubset ipmxSubset, System.Action<ISubset> drawAction)
        {
            context.CurrentRenderDepthStencilTarget = (isDefaultTarget ? context.CurrentTargetContext.DepthTargetView : stencilView);
            context.DeviceManager.Context.OutputMerger.SetTargets(context.CurrentRenderDepthStencilTarget, context.CurrentRenderColorTargets);
        }
    }
}
