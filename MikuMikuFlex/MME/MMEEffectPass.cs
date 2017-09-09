using MMF.MME.Script;
using MMF.Model;
using SlimDX.Direct3D11;

namespace MMF.MME
{
    public class MMEEffectPass
    {
        private RenderContext context;

        public string Command
        {
            get;
            private set;
        }

        public ScriptRuntime ScriptRuntime
        {
            get;
            private set;
        }

        public EffectPass Pass
        {
            get;
            private set;
        }

        public MMEEffectPass(RenderContext context, MMEEffectManager manager, EffectPass pass)
        {
            this.context = context;
            Pass = pass;
            EffectVariable annotation = EffectParseHelper.getAnnotation(pass, "Script", "string");
            Command = ((annotation == null) ? "" : annotation.AsString().GetString());
            if (!pass.VertexShaderDescription.Variable.IsValid)
            {
            }
            if (!pass.PixelShaderDescription.Variable.IsValid)
            {
            }
            ScriptRuntime = new ScriptRuntime(Command, context, manager, this);
        }

        public void Execute(System.Action<ISubset> drawAction, ISubset ipmxSubset)
        {
            if (string.IsNullOrWhiteSpace(ScriptRuntime.ScriptCode))
            {
                Pass.Apply(context.DeviceManager.Context);
                drawAction(ipmxSubset);
            }
            else
            {
                ScriptRuntime.Execute(drawAction, ipmxSubset);
            }
        }
    }
}
