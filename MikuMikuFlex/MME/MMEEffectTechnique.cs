using MMF.MME.Script;
using MMF.Model;
using SlimDX.Direct3D11;
using System.Collections.Generic;

namespace MMF.MME
{
    public class MMEEffectTechnique
    {
        public HashSet<int> Subset
        {
            get;
            private set;
        }

        public Dictionary<string, MMEEffectPass> Passes
        {
            get;
            private set;
        }

        public ScriptRuntime ScriptRuntime
        {
            get;
            private set;
        }

        public MMEEffectPassType MMDPassAnnotation
        {
            get;
            private set;
        }

        public ExtendedBoolean UseSphereMap
        {
            get;
            private set;
        }

        public ExtendedBoolean UseTexture
        {
            get;
            private set;
        }

        public ExtendedBoolean UseToon
        {
            get;
            private set;
        }

        public ExtendedBoolean UseSelfShadow
        {
            get;
            private set;
        }

        public ExtendedBoolean MulSphere
        {
            get;
            private set;
        }

        public MMEEffectTechnique(MMEEffectManager manager, EffectTechnique technique, int subsetCount, RenderContext context)
        {
            Subset = new HashSet<int>();
            Passes = new Dictionary<string, MMEEffectPass>();
            if (!technique.IsValid)
            {
                throw new InvalidMMEEffectShaderException(string.Format("テクニック「{0}」の検証に失敗しました。", technique.Description.Name));
            }
            string text = EffectParseHelper.getAnnotationString(technique, "MMDPass");
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.ToLower();
                string text2 = text;
                if (text2 != null)
                {
                    if (!(text2 == "object"))
                    {
                        if (!(text2 == "object_ss"))
                        {
                            if (!(text2 == "zplot"))
                            {
                                if (!(text2 == "shadow"))
                                {
                                    if (!(text2 == "edge"))
                                    {
                                        goto MYFAVORITEVOCALOIDISMIKU;
                                    }
                                    MMDPassAnnotation = MMEEffectPassType.Edge;
                                }
                                else
                                {
                                    MMDPassAnnotation = MMEEffectPassType.Shadow;
                                }
                            }
                            else
                            {
                                MMDPassAnnotation = MMEEffectPassType.ZPlot;
                            }
                        }
                        else
                        {
                            MMDPassAnnotation = MMEEffectPassType.Object_SelfShadow;
                        }
                    }
                    else
                    {
                        MMDPassAnnotation = MMEEffectPassType.Object;
                    }
                    goto HAVEANICEDAY;
                }
                MYFAVORITEVOCALOIDISMIKU:
                throw new System.InvalidOperationException("予期しない識別子");
            }
            MMDPassAnnotation = MMEEffectPassType.Object;
            HAVEANICEDAY:
            UseTexture = EffectParseHelper.getAnnotationBoolean(technique, "UseTexture");
            UseSphereMap = EffectParseHelper.getAnnotationBoolean(technique, "UseSphereMap");
            UseToon = EffectParseHelper.getAnnotationBoolean(technique, "UseToon");
            UseSelfShadow = EffectParseHelper.getAnnotationBoolean(technique, "UseSelfShadow");
            MulSphere = EffectParseHelper.getAnnotationBoolean(technique, "MulSphere");
            GetSubsets(technique, subsetCount);
            EffectVariable annotation = EffectParseHelper.getAnnotation(technique, "Script", "string");
            for (int i = 0; i < technique.Description.PassCount; i++)
            {
                EffectPass passByIndex = technique.GetPassByIndex(i);
                Passes.Add(passByIndex.Description.Name, new MMEEffectPass(context, manager, passByIndex));
            }
            if (annotation != null)
            {
                ScriptRuntime = new ScriptRuntime(annotation.AsString().GetString(), context, manager, this);
            }
            else
            {
                ScriptRuntime = new ScriptRuntime("", context, manager, this);
            }
        }

        private void GetSubsets(EffectTechnique technique, int subsetCount)
        {
            string annotationString = EffectParseHelper.getAnnotationString(technique, "Subset");
            if (string.IsNullOrWhiteSpace(annotationString))
            {
                for (int i = 0; i <= subsetCount; i++)
                {
                    Subset.Add(i);
                }
            }
            else
            {
                string[] array = annotationString.Split(new char[]
                {
                    ','
                });
                string[] array2 = array;
                for (int j = 0; j < array2.Length; j++)
                {
                    string text = array2[j];
                    if (text.IndexOf('-') == -1)
                    {
                        int num = 0;
                        if (!int.TryParse(text, out num))
                        {
                            throw new InvalidMMEEffectShaderException(string.Format("テクニック「{0}」のサブセット解析中にエラーが発生しました。「{1}」中の「{2}」は認識されません。", technique.Description.Name, annotationString, text));
                        }
                        Subset.Add(num);
                    }
                    else
                    {
                        string[] array3 = text.Split(new char[]
                        {
                            '-'
                        });
                        if (array3.Length > 2)
                        {
                            throw new InvalidMMEEffectShaderException(string.Format("テクニック「{0}」のサブセット解析中にエラーが発生しました。「{1}」中の「{2}」には\"-\"が2つ以上存在します。", technique.Description.Name, annotationString, text));
                        }
                        if (string.IsNullOrWhiteSpace(array3[1]))
                        {
                            int num = 0;
                            if (!int.TryParse(array3[0], out num))
                            {
                                throw new InvalidMMEEffectShaderException(string.Format("テクニック「{0}」のサブセット解析中にエラーが発生しました。「{1}」中の「{2}」の「{3}」は認識されません。", new object[]
                                {
                                    technique.Description.Name,
                                    annotationString,
                                    text,
                                    array3[0]
                                }));
                            }
                            for (int i = num; i <= subsetCount; i++)
                            {
                                Subset.Add(i);
                            }
                        }
                        else
                        {
                            int num2 = 0;
                            int num3 = 0;
                            if (!int.TryParse(array3[0], out num2) || !int.TryParse(array3[1], out num3))
                            {
                                throw new InvalidMMEEffectShaderException(string.Format("テクニック「{0}」のサブセット解析中にエラーが発生しました。「{1}」中の「{2}」の「{3}」もしくは「{4}」は認識されません。", new object[]
                                {
                                    technique.Description.Name,
                                    annotationString,
                                    text,
                                    array3[0],
                                    array3[1]
                                }));
                            }
                            for (int i = num2; i <= num3; i++)
                            {
                                Subset.Add(i);
                            }
                        }
                    }
                }
            }
        }

        public void ExecuteTechnique(DeviceContext context, System.Action<ISubset> drawAction, ISubset ipmxSubset)
        {
            if (string.IsNullOrWhiteSpace(ScriptRuntime.ScriptCode))
            {
                foreach (MMEEffectPass current in Passes.Values)
                {
                    current.Pass.Apply(context);
                    drawAction(ipmxSubset);
                }
            }
            else
            {
                ScriptRuntime.Execute(drawAction, ipmxSubset);
            }
        }

        public static bool CheckExtebdedBoolean(ExtendedBoolean teqValue, bool subsetValue)
        {
            bool result;
            if (subsetValue)
            {
                result = (teqValue != ExtendedBoolean.Disable);
            }
            else
            {
                result = (teqValue != ExtendedBoolean.Enable);
            }
            return result;
        }
    }
}
