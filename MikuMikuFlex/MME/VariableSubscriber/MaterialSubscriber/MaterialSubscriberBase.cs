using System.Linq;
using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    public abstract class MaterialSubscriberBase : SubscriberBase
    {
        private static readonly string[] NeedAnnotation = new string[]
        {"DIFFUSE", "AMBIENT", "SPECULAR", "EDGECOLOR", "GROUNDSHADOWCOLOR"};

        protected bool IsVector3;
        protected TargetObject Target;

        protected MaterialSubscriberBase(TargetObject target, bool isVector3)
        {
            Target = target;
            IsVector3 = isVector3;
        }

        protected MaterialSubscriberBase()
        {
        }

        public override VariableType[] Types
        {
            get
            {
                return Semantics == "SPECULARPOWER"||Semantics=="EDGETHICKNESS"
                    ? new[] {VariableType.Float}
                    : new[] {VariableType.Float3, VariableType.Float4};
            }
        }

        public override UpdateBy UpdateTiming
        {
            get { return UpdateBy.Material; }
        }

        protected void SetAsVector(Vector3 vec, SlimDX.Direct3D11.Effect effect, int index)
        {
            effect.GetVariableByIndex(index).AsVector().Set(vec);
        }

        protected void SetAsVector(Vector4 vec, SlimDX.Direct3D11.Effect effect, int index, bool isVector3)
        {
            if (!isVector3)
            {
                effect.GetVariableByIndex(index).AsVector().Set(vec);
            }
            else
            {
                effect.GetVariableByIndex(index).AsVector().Set(new Vector3(vec.X, vec.Y, vec.Z));
            }
        }

        protected void SetAsVector(Vector4 vec, EffectVariable variable, bool isVector3)
        {
            if (!isVector3)
            {
                variable.AsVector().Set(vec);
            }
            else
            {
                variable.AsVector().Set(new Vector3(vec.X, vec.Y, vec.Z));
            }
        }

        protected void SetAsFloat(float val, SlimDX.Direct3D11.Effect effect, int index)
        {
            effect.GetVariableByIndex(index).AsScalar().Set(val);
        }

        protected void SetAsFloat(float val, EffectVariable variable)
        {
            variable.AsScalar().Set(val);
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            bool isVector3 =
                variable.GetVariableType().Description.TypeName.ToLower().Equals("float3");
            if (!NeedAnnotation.Contains(Semantics))
            {
                return GetSubscriberInstance(TargetObject.UnUsed, isVector3);
            }
            EffectVariable objectAnnotation = EffectParseHelper.getAnnotation(variable, "Object", "string");
            if (objectAnnotation == null)
            {
                throw new InvalidMMEEffectShaderException(
                    string.Format("このセマンティクス\"{0}\"にはアノテーション「Object」が必須ですが、記述されませんでした。", Semantics));
            }
            string annotation = objectAnnotation.AsString().GetString().ToLower();
            if (!string.IsNullOrWhiteSpace(annotation))
            {
                switch (annotation)
                {
                    case "geometry":
                        return GetSubscriberInstance(TargetObject.Geometry, isVector3);
                    case "light":
                        return GetSubscriberInstance(TargetObject.Light, isVector3);
                    default:
                        throw new InvalidMMEEffectShaderException(string.Format("アノテーション\"{0}\"は認識されません。", annotation));
                }
            }
            throw new InvalidMMEEffectShaderException(
                string.Format("このセマンティクス\"{0}\"にはアノテーション「Object」が必須ですが、記述されませんでした。", Semantics));
        }

        protected abstract SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3);
    }
}