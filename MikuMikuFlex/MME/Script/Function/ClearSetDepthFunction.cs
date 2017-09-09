using MMF.Model;
using SlimDX.Direct3D11;

namespace MMF.MME.Script.Function
{
    internal class ClearSetDepthFunction : FunctionBase
    {
        private RenderContext context;

        private EffectVariable sourceVariable;

        public override string FunctionName
        {
            get
            {
                return "ClearSetDepth";
            }
        }

        public override FunctionBase GetExecuterInstance(int index, string value, RenderContext context, ScriptRuntime runtime, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            ClearSetDepthFunction clearSetDepthFunction = new ClearSetDepthFunction();
            clearSetDepthFunction.context = context;
            clearSetDepthFunction.sourceVariable = manager.EffectFile.GetVariableByName(value);
            if (clearSetDepthFunction.sourceVariable == null)
            {
                throw new InvalidMMEEffectShaderException(string.Format("ClearSetDepth={0};が指定されましたが、変数\"{0}\"は見つかりませんでした。", value));
            }
            if (!clearSetDepthFunction.sourceVariable.GetVariableType().Description.TypeName.ToLower().Equals("float"))
            {
                throw new InvalidMMEEffectShaderException(string.Format("ClearSetDepth={0};が指定されましたが、変数\"{0}\"はfloat型ではありません。", value));
            }
            return clearSetDepthFunction;
        }

        public override void Execute(ISubset ipmxSubset, System.Action<ISubset> drawAction)
        {
            context.CurrentClearDepth = sourceVariable.AsScalar().GetFloat();
        }
    }
}
