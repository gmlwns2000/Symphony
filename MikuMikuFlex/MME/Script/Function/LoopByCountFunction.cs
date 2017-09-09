using MMF.Model;
using SlimDX.Direct3D11;

namespace MMF.MME.Script.Function
{
    internal class LoopByCountFunction : FunctionBase
    {
        private ScriptRuntime runtime;

        private int loopCount;

        public override string FunctionName
        {
            get
            {
                return "LoopByCount";
            }
        }

        public override FunctionBase GetExecuterInstance(int index, string value, RenderContext context, ScriptRuntime runtime, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            LoopByCountFunction loopByCountFunction = new LoopByCountFunction();
            loopByCountFunction.runtime = runtime;
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidMMEEffectShaderException("LoopByCount=;は指定できません。int,float,boolいずれかの変数名を伴う必要があります。");
            }
            EffectVariable variableByName = manager.EffectFile.GetVariableByName(value);
            string text = variableByName.GetVariableType().Description.TypeName.ToLower();
            string text2 = text;
            if (text2 != null)
            {
                int num;
                if (!(text2 == "bool") && !(text2 == "int"))
                {
                    if (!(text2 == "float"))
                    {
                        goto THANKYOU;
                    }
                    num = (int)variableByName.AsScalar().GetFloat();
                }
                else
                {
                    num = variableByName.AsScalar().GetInt();
                }
                loopByCountFunction.loopCount = num;
                return loopByCountFunction;
            }
            THANKYOU:
            throw new InvalidMMEEffectShaderException("LoopByCountに指定できる変数の型はfloat,int,boolのいずれかです。");
        }

        public override void Execute(ISubset ipmxSubset, System.Action<ISubset> drawAction)
        {
            runtime.LoopBegins.Push(runtime.ParsedExecuters.Count);
            runtime.LoopCounts.Push(0);
            runtime.LoopEndCount.Push(loopCount);
        }
    }
}
