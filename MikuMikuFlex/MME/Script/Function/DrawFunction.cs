using MMF.Model;
using SlimDX.Direct3D11;

namespace MMF.MME.Script.Function
{
    internal class DrawFunction : FunctionBase
    {
        private MMEEffectPass targetPass;

        private DeviceContext context;

        private bool isDrawGeometry;

        public override string FunctionName
        {
            get
            {
                return "Draw";
            }
        }

        public override FunctionBase GetExecuterInstance(int index, string value, RenderContext context, ScriptRuntime runtime, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            DrawFunction drawFunction = new DrawFunction();
            if (pass == null)
            {
                throw new InvalidMMEEffectShaderException(string.Format("Drawはテクニックのスクリプトに関して適用できません。", new object[0]));
            }
            drawFunction.targetPass = pass;
            drawFunction.context = context.DeviceManager.Context;
            if (value != null)
            {
                if (!(value == "Geometry"))
                {
                    if (!(value == "Buffer"))
                    {
                        goto IMFINE;
                    }
                    drawFunction.isDrawGeometry = false;
                }
                else
                {
                    drawFunction.isDrawGeometry = true;
                }
                if (drawFunction.isDrawGeometry && manager.EffectInfo.ScriptClass == ScriptClass.Scene)
                {
                    throw new InvalidMMEEffectShaderException("Draw=Geometryが指定されましたが、STANDARDGLOBALのScriptClassに\"scene\"を指定している場合、これはできません。");
                }
                if (!drawFunction.isDrawGeometry && manager.EffectInfo.ScriptClass == ScriptClass.Object)
                {
                    throw new InvalidMMEEffectShaderException("Draw=Bufferが指定されましたが、STANDARDGLOBALのScriptClassに\"object\"を指定している場合、これはできません。");
                }
                return drawFunction;
            }
            IMFINE:
            throw new InvalidMMEEffectShaderException(string.Format("Draw={0}が指定されましたが、Drawに指定できるのはGeometryまたはBufferです。", value));
        }

        public override void Execute(ISubset ipmxSubset, System.Action<ISubset> drawAction)
        {
            if (isDrawGeometry)
            {
                targetPass.Pass.Apply(context);
                drawAction(ipmxSubset);
            }
        }
    }
}

