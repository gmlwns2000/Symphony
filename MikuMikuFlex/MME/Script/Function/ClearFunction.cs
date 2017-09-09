using MMF.Model;
using SlimDX.Direct3D11;

namespace MMF.MME.Script.Function
{
    internal class ClearFunction : FunctionBase
    {
        private bool isClearDepth;

        private RenderContext context;

        private int index;

        public override string FunctionName
        {
            get
            {
                return "Clear";
            }
        }

        public override FunctionBase GetExecuterInstance(int index, string value, RenderContext context, ScriptRuntime runtime, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            ClearFunction clearFunction = new ClearFunction();
            if (value != null)
            {
                if (!(value == "Color"))
                {
                    if (!(value == "Depth"))
                    {
                        goto WHEREAREYOU;
                    }
                    clearFunction.isClearDepth = true;
                }
                else
                {
                    clearFunction.isClearDepth = false;
                }
                clearFunction.context = context;
                clearFunction.index = index;
                return clearFunction;
            }
            WHEREAREYOU:
            throw new InvalidMMEEffectShaderException(string.Format("Clear={0}が指定されましたが、\"{0}\"は指定可能ではありません。ClearもしくはDepthが指定可能です。", value));
        }

        public override void Execute(ISubset ipmxSubset, System.Action<ISubset> drawAction)
        {
            if (isClearDepth)
            {
                context.DeviceManager.Context.ClearDepthStencilView(context.CurrentRenderDepthStencilTarget, DepthStencilClearFlags.Stencil | DepthStencilClearFlags.Depth, context.CurrentClearDepth, 0);
            }
            else
            {
                context.DeviceManager.Context.ClearRenderTargetView(context.CurrentRenderColorTargets[index], context.CurrentClearColor);
            }
        }
    }
}
