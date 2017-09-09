using SlimDX.Direct3D11;
using System.Linq;

namespace MMF.MME
{
    public static class EffectParseHelper
    {
        public static EffectVariable getAnnotation(EffectVariable variable, string target, string typeName)
        {
            string text = target.ToLower();
            string[] array = text.Split(new char[]
            {
                '/'
            });
            int i = 0;
            EffectVariable result;
            while (i < variable.Description.AnnotationCount)
            {
                EffectVariable annotationByIndex = variable.GetAnnotationByIndex(i);
                string text2 = annotationByIndex.Description.Name.ToLower();
                if (text2 == text)
                {
                    if (!array.Contains(text2) && !string.IsNullOrWhiteSpace(text2))
                    {
                        throw new InvalidMMEEffectShaderException(string.Format("変数「{0} {1}:{2}」に適用されたアノテーション「{3} {4}」はアノテーションの型が正しくありません。期待した型は{5}でした。", new object[]
                        {
                            variable.GetVariableType().Description.TypeName,
                            variable.Description.Name,
                            variable.Description.Semantic,
                            annotationByIndex.GetVariableType().Description.TypeName,
                            annotationByIndex.Description.Name,
                            EffectParseHelper.getExpectedTypes(array, annotationByIndex.Description.Name)
                        }));
                    }
                    result = annotationByIndex;
                    return result;
                }
                else
                {
                    i++;
                }
            }
            result = null;
            return result;
        }

        public static EffectVariable getAnnotation(EffectPass pass, string target, string typeName)
        {
            string text = target.ToLower();
            string[] array = text.Split(new char[]
            {
                '/'
            });
            int i = 0;
            EffectVariable result;
            while (i < pass.Description.AnnotationCount)
            {
                EffectVariable annotationByIndex = pass.GetAnnotationByIndex(i);
                string text2 = annotationByIndex.Description.Name.ToLower();
                if (text2 == text)
                {
                    if (!array.Contains(text2) && !string.IsNullOrWhiteSpace(text2))
                    {
                        throw new InvalidMMEEffectShaderException(string.Format("パス「{0}」に適用されたアノテーション「{1} {2}」はアノテーションの型が正しくありません。期待した型は{3}でした。", new object[]
                        {
                            pass.Description.Name,
                            text2,
                            annotationByIndex.Description.Name,
                            EffectParseHelper.getExpectedTypes(array, annotationByIndex.Description.Name)
                        }));
                    }
                    result = annotationByIndex;
                    return result;
                }
                else
                {
                    i++;
                }
            }
            result = null;
            return result;
        }

        public static EffectVariable getAnnotation(EffectTechnique technique, string target, string typeName)
        {
            string text = target.ToLower();
            string[] array = text.Split(new char[]
            {
                '/'
            });
            int i = 0;
            EffectVariable result;
            while (i < technique.Description.AnnotationCount)
            {
                EffectVariable annotationByIndex = technique.GetAnnotationByIndex(i);
                string text2 = annotationByIndex.Description.Name.ToLower();
                if (text2 == text)
                {
                    if (!array.Contains(text2) && !string.IsNullOrWhiteSpace(text2))
                    {
                        throw new InvalidMMEEffectShaderException(string.Format("テクニック「{0}」に適用されたアノテーション「{1} {2}」はアノテーションの型が正しくありません。期待した型は{3}でした。", new object[]
                        {
                            technique.Description.Name,
                            text2,
                            annotationByIndex.Description.Name,
                            EffectParseHelper.getExpectedTypes(array, annotationByIndex.Description.Name)
                        }));
                    }
                    result = annotationByIndex;
                    return result;
                }
                else
                {
                    i++;
                }
            }
            result = null;
            return result;
        }

        public static EffectVariable getAnnotation(EffectGroup group, string target, string typeName)
        {
            string text = target.ToLower();
            string[] array = text.Split(new char[]
            {
                '/'
            });
            int i = 0;
            EffectVariable result;
            while (i < group.Description.AnnotationCount)
            {
                EffectVariable annotationByIndex = group.GetAnnotationByIndex(i);
                string text2 = annotationByIndex.Description.Name.ToLower();
                if (text2 == text)
                {
                    if (!array.Contains(text2) && !string.IsNullOrWhiteSpace(text2))
                    {
                        throw new InvalidMMEEffectShaderException(string.Format("エフェクトグループ「{0}」に適用されたアノテーション「{1} {2}」はアノテーションの型が正しくありません。期待した型は{3}でした。", new object[]
                        {
                            group.Description.Name,
                            text2,
                            annotationByIndex.Description.Name,
                            EffectParseHelper.getExpectedTypes(array, annotationByIndex.Description.Name)
                        }));
                    }
                    result = annotationByIndex;
                    return result;
                }
                else
                {
                    i++;
                }
            }
            result = null;
            return result;
        }

        public static string getAnnotationString(EffectTechnique technique, string attrName)
        {
            EffectVariable annotation = EffectParseHelper.getAnnotation(technique, attrName, "string");
            string result;
            if (annotation == null)
            {
                result = "";
            }
            else
            {
                result = annotation.AsString().GetString();
            }
            return result;
        }

        public static ExtendedBoolean getAnnotationBoolean(EffectTechnique technique, string attrName)
        {
            EffectVariable annotation = EffectParseHelper.getAnnotation(technique, attrName, "bool");
            ExtendedBoolean result;
            if (annotation == null)
            {
                result = ExtendedBoolean.Ignore;
            }
            else
            {
                int @int = annotation.AsScalar().GetInt();
                if (@int == 1)
                {
                    result = ExtendedBoolean.Enable;
                }
                else
                {
                    result = ExtendedBoolean.Disable;
                }
            }
            return result;
        }

        private static string getExpectedTypes(string[] types, string name)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < types.Length; i++)
            {
                string text = types[i];
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.Append(",");
                    }
                    stringBuilder.Append(string.Format("「{0} {1}」", text, name));
                }
            }
            return stringBuilder.ToString();
        }
    }
}