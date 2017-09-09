using MMF.Model;

namespace MMF.MME.Script
{
    internal abstract class FunctionBase
    {
        public abstract string FunctionName
        {
            get;
        }

        public abstract FunctionBase GetExecuterInstance(int index, string value, RenderContext context, ScriptRuntime runtime, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass);

        public abstract void Execute(ISubset subset, System.Action<ISubset> action);

        public virtual void Increment(ScriptRuntime runtime)
        {
            runtime.CurrentExecuter++;
        }
    }
}
