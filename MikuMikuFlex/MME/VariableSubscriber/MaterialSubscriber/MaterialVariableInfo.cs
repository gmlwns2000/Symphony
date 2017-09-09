using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    public class MaterialVariableInfo
    {
        public MaterialVariableInfo(string semantics, EffectVariable variable)
        {
            if (semantics == "SPECULARPOWER" || semantics == "EMISSIVE" || semantics == "TOONCOLOR")
            {
                Target = TargetObject.UnUsed;
                VariableType = variable.GetVariableType();
            }
        }

        public EffectType VariableType { get; private set; }

        public TargetObject Target { get; private set; }
    }
}