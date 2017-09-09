using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    public abstract class MatrixSubscriberBase : SubscriberBase
    {
        protected ObjectAnnotationType TargetObject;

        public override VariableType[] Types
        {
            get
            {
                return new VariableType[1];
            }
        }

        protected MatrixSubscriberBase(ObjectAnnotationType Object)
        {
            TargetObject = Object;
        }

        protected MatrixSubscriberBase()
        {
        }

        protected void SetAsMatrix(Matrix matrix, EffectVariable variable)
        {
            variable.AsMatrix().SetMatrix(matrix);
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            EffectVariable annotation = EffectParseHelper.getAnnotation(variable, "Object", "string");
            string text = (annotation == null) ? "" : annotation.AsString().GetString();
            SubscriberBase subscriberInstance;
            if (!string.IsNullOrWhiteSpace(text))
            {
                string text2 = text.ToLower();
                if (text2 != null)
                {
                    if (text2 == "camera")
                    {
                        subscriberInstance = GetSubscriberInstance(ObjectAnnotationType.Camera);
                        return subscriberInstance;
                    }
                    if (text2 == "light")
                    {
                        subscriberInstance = GetSubscriberInstance(ObjectAnnotationType.Light);
                        return subscriberInstance;
                    }
                    if (text2 == "")
                    {
                        throw new InvalidMMEEffectShaderException(string.Format("変数「{0} {1}:{2}」には、アノテーション「string Object=\"Camera\"」または、「string Object=\"Light\"」が必須ですが指定されませんでした。", variable.GetVariableType().Description.TypeName.ToLower(), variable.Description.Name, variable.Description.Semantic));
                    }
                }
                throw new InvalidMMEEffectShaderException(string.Format("変数「{0} {1}:{2}」には、アノテーション「string Object=\"Camera\"」または、「string Object=\"Light\"」が必須ですが指定されたのは「string Object=\"{3}\"」でした。(スペルミス?)", new object[]
                {
                    variable.GetVariableType().Description.TypeName.ToLower(),
                    variable.Description.Name,
                    variable.Description.Semantic,
                    text
                }));
            }
            subscriberInstance = GetSubscriberInstance(ObjectAnnotationType.Camera);
            return subscriberInstance;
        }

        protected abstract SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object);
    }
}
