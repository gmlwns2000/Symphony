using MMF.Model;
using SlimDX.Direct3D11;

namespace MMF.MME.Script.Function
{
    internal class RenderColorTargetFunction : FunctionBase
    {
        private int index;

        private RenderContext context;

        private RenderTargetView targetView;

        private bool isDefaultTarget;

        public override string FunctionName
        {
            get
            {
                return "RenderColorTarget";
            }
        }

        public override FunctionBase GetExecuterInstance(int index, string value, RenderContext context, ScriptRuntime runtime, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            RenderColorTargetFunction renderColorTargetFunction = new RenderColorTargetFunction();
            if (index < 0 || index > 7)
            {
                throw new InvalidMMEEffectShaderException("RenderColorTarget[n](0<=n<=7)のnの制約が満たされていません。");
            }
            renderColorTargetFunction.index = index;
            renderColorTargetFunction.context = context;
            if (!string.IsNullOrWhiteSpace(value) && !manager.RenderColorTargetViewes.ContainsKey(value))
            {
                throw new InvalidMMEEffectShaderException("指定されたRENDERCOLORTARGETの変数は存在しません。");
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                renderColorTargetFunction.isDefaultTarget = true;
            }
            renderColorTargetFunction.targetView = (string.IsNullOrWhiteSpace(value) ? ((index == 0) ? context.CurrentTargetContext.RenderTargetView : null) : manager.RenderColorTargetViewes[value]);
            return renderColorTargetFunction;
        }

        public override void Execute(ISubset ipmxSubset, System.Action<ISubset> drawAction)
        {
            context.CurrentRenderColorTargets[index] = (isDefaultTarget ? context.CurrentTargetContext.RenderTargetView : targetView);
            context.DeviceManager.Context.OutputMerger.SetTargets(context.CurrentRenderDepthStencilTarget, context.CurrentRenderColorTargets);
        }
    }
}
