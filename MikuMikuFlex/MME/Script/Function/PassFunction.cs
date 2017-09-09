using MMF.Model;

namespace MMF.MME.Script.Function
{
    internal class PassFunction : FunctionBase
    {
        private MMEEffectPass targetPass;

        public override string FunctionName
        {
            get
            {
                return "Pass";
            }
        }

        public override FunctionBase GetExecuterInstance(int index, string value, RenderContext context, ScriptRuntime runtime, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            PassFunction passFunction = new PassFunction();
            if (technique == null)
            {
                throw new InvalidMMEEffectShaderException(string.Format("スクリプト内でPassを利用できるのはテクニックに適用されたスクリプトのみです。パスのスクリプトでは実行できません。", new object[0]));
            }
            if (!technique.Passes.ContainsKey(value))
            {
                throw new InvalidMMEEffectShaderException(string.Format("スクリプトで指定されたテクニック中では指定されたパス\"{0}\"は見つかりませんでした。(スペルミス?)", value));
            }
            passFunction.targetPass = technique.Passes[value];
            return passFunction;
        }

        public override void Execute(ISubset ipmxSubset, System.Action<ISubset> drawAction)
        {
            targetPass.Execute(drawAction, ipmxSubset);
        }
    }
}
