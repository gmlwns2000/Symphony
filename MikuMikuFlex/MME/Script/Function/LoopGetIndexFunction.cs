using MMF.Model;
using SlimDX.Direct3D11;

namespace MMF.MME.Script.Function
{
    internal class LoopGetIndexFunction : FunctionBase
    {
        private EffectVariable targetVariable;

        private ScriptRuntime runtime;

        public override string FunctionName
        {
            get
            {
                return "LoopGetIndex";
            }
        }

        public override FunctionBase GetExecuterInstance(int index, string value, RenderContext context, ScriptRuntime runtime, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            return new LoopGetIndexFunction
            {
                targetVariable = manager.EffectFile.GetVariableByName(value),
                runtime = runtime
            };
        }

        public override void Execute(ISubset ipmxSubset, System.Action<ISubset> drawAction)
        {
            int num = runtime.LoopCounts.Pop();
            targetVariable.AsScalar().Set(num);
            runtime.LoopCounts.Push(num);
        }
    }
}
