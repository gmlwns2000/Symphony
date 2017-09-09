using MMF.Model;
using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.Script.Function
{
    internal class ClearSetColorFunction : FunctionBase
    {
        private EffectVariable sourceVariable;

        private RenderContext context;

        public override string FunctionName
        {
            get
            {
                return "ClearSetColor";
            }
        }

        public override FunctionBase GetExecuterInstance(int index, string value, RenderContext context, ScriptRuntime runtime, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            ClearSetColorFunction clearSetColorFunction = new ClearSetColorFunction();
            clearSetColorFunction.sourceVariable = manager.EffectFile.GetVariableByName(value);
            clearSetColorFunction.context = context;
            if (clearSetColorFunction.sourceVariable == null)
            {
                throw new InvalidMMEEffectShaderException(string.Format("ClearSetColor={0};が指定されましたが、変数\"{0}\"は見つかりませんでした。", value));
            }
            if (!clearSetColorFunction.sourceVariable.GetVariableType().Description.TypeName.ToLower().Equals("float4"))
            {
                throw new InvalidMMEEffectShaderException(string.Format("ClearSetColor={0};が指定されましたが、変数\"{0}\"はfloat4型ではありません。", value));
            }
            return clearSetColorFunction;
        }

        public override void Execute(ISubset ipmxSubset, System.Action<ISubset> drawAction)
        {
            context.CurrentClearColor = new Color4(sourceVariable.AsVector().GetVector());
        }
    }
}
