using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.WorldInfoSubscriber
{
    public abstract class WorldInfoSubscriberBase : SubscriberBase
    {
        internal ObjectAnnotationType Object;

        protected WorldInfoSubscriberBase(ObjectAnnotationType obj)
        {
            Object = obj;
        }

        internal WorldInfoSubscriberBase()
        {
        }


        public override VariableType[] Types
        {
            get { return new VariableType[2] {VariableType.Float3, VariableType.Float4}; }
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            EffectVariable annotation = EffectParseHelper.getAnnotation(variable, "Object", "string");
            if (annotation == null || string.IsNullOrWhiteSpace(annotation.AsString().GetString()))
            {
                throw new InvalidMMEEffectShaderException(
                    string.Format(
                        "変数「{0} {1}:{2}」にはアノテーション「string Object=\"Light\"」または「string object=\"Camera\"」が必須ですが指定されませんでした。",
                        variable.GetVariableType().Description.TypeName.ToLower(), variable.Description.Name,
                        variable.Description.Semantic));
            }
            string objectStr = annotation.AsString().GetString().ToLower();
            ObjectAnnotationType type;
            switch (objectStr)
            {
                case "camera":
                    type = ObjectAnnotationType.Camera;
                    break;
                case "light":
                    type = ObjectAnnotationType.Light;
                    break;
                default:
                    throw new InvalidMMEEffectShaderException(
                        string.Format(
                            "変数「{0} {1}:{2}」にはアノテーション「string Object=\"Light\"」または「string object=\"Camera\"」が必須ですが指定されたのは,「string object=\"{3}\"でした。(スペルミス?)"
                            , variable.GetVariableType().Description.TypeName.ToLower(), variable.Description.Name,
                            variable.Description.Semantic, objectStr));
            }
            return GetSubscriberInstance(type);
        }

        protected abstract SubscriberBase GetSubscriberInstance(ObjectAnnotationType type);
    }
}