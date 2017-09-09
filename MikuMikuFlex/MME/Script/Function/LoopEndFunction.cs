using MMF.Model;

namespace MMF.MME.Script.Function
{
    internal class LoopEndFunction : FunctionBase
    {
        public override string FunctionName
        {
            get
            {
                return "LoopEnd";
            }
        }

        public override FunctionBase GetExecuterInstance(int index, string value, RenderContext context, ScriptRuntime runtime, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            return new LoopEndFunction();
        }

        public override void Execute(ISubset ipmxSubset, System.Action<ISubset> drawAction)
        {
        }

        public override void Increment(ScriptRuntime runtime)
        {
            int num = runtime.LoopEndCount.Pop();
            int num2 = runtime.LoopCounts.Pop();
            int num3 = runtime.LoopBegins.Pop();
            if (num2 < num)
            {
                runtime.CurrentExecuter = num3 + 1;
                runtime.LoopCounts.Push(num2 + 1);
                runtime.LoopEndCount.Push(num);
                runtime.LoopBegins.Push(num3);
            }
            else
            {
                runtime.CurrentExecuter++;
            }
        }
    }
}
