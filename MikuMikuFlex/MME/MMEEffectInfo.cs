using SlimDX.Direct3D11;

namespace MMF.MME
{
    public class MMEEffectInfo
    {
        public string ScriptOutput
        {
            get;
            private set;
        }

        public ScriptClass ScriptClass
        {
            get;
            private set;
        }

        public ScriptOrder ScriptOrder
        {
            get;
            private set;
        }

        public string StandardGlobalScript
        {
            get;
            private set;
        }

        public System.Collections.Generic.List<EffectTechnique> SortedTechnique
        {
            get;
            private set;
        }

        public MMEEffectInfo(Effect effect)
        {
            ScriptOutput = "color";
            ScriptClass = ScriptClass.Object;
            ScriptOrder = ScriptOrder.Standard;
            StandardGlobalScript = "";
            SortedTechnique = new System.Collections.Generic.List<EffectTechnique>();
            for (int i = 0; i < effect.Description.GlobalVariableCount; i++)
            {
                EffectVariable variableByIndex = effect.GetVariableByIndex(i);
                if (variableByIndex.Description.Semantic.ToUpper().Equals("STANDARDGLOBAL"))
                {
                    ParseStandardGlobal(effect, variableByIndex);
                    break;
                }
            }
        }

        private void ParseStandardGlobal(Effect effect, EffectVariable sg)
        {
            if (!sg.Description.Name.ToLower().Equals("script"))
            {
                throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される変数名は\"Script\"である必要があります、指定された変数名は\"{0}\"でした。", sg.Description.Name));
            }
            if (!sg.GetVariableType().Description.TypeName.ToLower().Equals("float"))
            {
                throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される変数型は\"float\"である必要があります、指定された変数名は\"{0}\"でした。", sg.GetVariableType().Description.TypeName.ToLower()));
            }
            if (sg.AsScalar().GetFloat() != 0.8f)
            {
                throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される値は\"0.8\"である必要があります、指定された値は\"{0}\"でした。", sg.AsScalar().GetFloat()));
            }
            EffectVariable annotation = EffectParseHelper.getAnnotation(sg, "ScriptOutput", "string");
            if (annotation != null)
            {
                if (!annotation.AsString().GetString().Equals("color"))
                {
                    throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される変数のアノテーション「string ScriptOutput」は、\"color\"でなくてはなりません。指定された値は\"{0}\"でした。", annotation.AsString().GetString().ToLower()));
                }
            }
            EffectVariable annotation2 = EffectParseHelper.getAnnotation(sg, "ScriptClass", "string");
            if (annotation2 != null)
            {
                string @string = annotation2.AsString().GetString();
                string text = @string.ToLower();
                if (text != null)
                {
                    if (!(text == "object"))
                    {
                        if (!(text == "scene"))
                        {
                            if (!(text == "sceneorobject"))
                            {
                                goto ILIKEPROGRAMMING;
                            }
                            ScriptClass = ScriptClass.SceneOrObject;
                        }
                        else
                        {
                            ScriptClass = ScriptClass.Scene;
                        }
                    }
                    else
                    {
                        ScriptClass = ScriptClass.Object;
                    }
                    goto HOWAREYOUTODAY;
                }
                ILIKEPROGRAMMING:
                throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される変数のアノテーション「string ScriptClass」は、\"object\",\"scene\",\"sceneorobject\"でなくてはなりません。指定された値は\"{0}\"でした。(スペルミス?)", @string.ToLower()));
            }
            HOWAREYOUTODAY:
            EffectVariable annotation3 = EffectParseHelper.getAnnotation(sg, "ScriptOrder", "string");
            if (annotation3 != null)
            {
                string string2 = annotation3.AsString().GetString();
                string text = string2.ToLower();
                if (text != null)
                {
                    if (!(text == "standard"))
                    {
                        if (!(text == "preprocess"))
                        {
                            if (!(text == "postprocess"))
                            {
                                goto BEHAPPY;
                            }
                            ScriptOrder = ScriptOrder.Postprocess;
                        }
                        else
                        {
                            ScriptOrder = ScriptOrder.Preprocess;
                        }
                    }
                    else
                    {
                        ScriptOrder = ScriptOrder.Standard;
                    }
                    goto DREAMSCOMETRUE;
                }
                BEHAPPY:
                throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される変数のアノテーション「string ScriptOrder」は、\"standard\",\"preprocess\",\"postprocess\"でなくてはなりません。指定された値は\"{0}\"でした。(スペルミス?)", string2.ToLower()));
            }
            DREAMSCOMETRUE:
            EffectVariable annotation4 = EffectParseHelper.getAnnotation(sg, "Script", "string");
            if (annotation4 != null)
            {
                StandardGlobalScript = annotation4.AsString().GetString();
                if (string.IsNullOrEmpty(StandardGlobalScript))
                {
                    for (int i = 0; i < effect.Description.TechniqueCount; i++)
                    {
                        SortedTechnique.Add(effect.GetTechniqueByIndex(i));
                    }
                }
                else
                {
                    string[] array = StandardGlobalScript.Split(new char[]
                    {
                        ';'
                    });
                    if (array.Length == 1)
                    {
                        throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される変数のスクリプト「{0}」は読み込めませんでした。\";\"が足りません。", StandardGlobalScript));
                    }
                    string text2 = array[array.Length - 2];
                    if (StandardGlobalScript.IndexOf("?") == -1)
                    {
                        string[] array2 = text2.Split(new char[]
                        {
                            '='
                        });
                        if (array2.Length > 2)
                        {
                            throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される変数のスクリプト「{0}」は読み込めませんでした。\"=\"の数が多すぎます。", text2));
                        }
                        if (!array2[0].ToLower().Equals("technique"))
                        {
                            throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される変数のスクリプト「{0}」は読み込めませんでした。\"{1}\"は\"Technique\"であるべきです。(スペルミス?)", text2, array2[0]));
                        }
                        EffectTechnique techniqueByName = effect.GetTechniqueByName(array2[1]);
                        if (techniqueByName == null)
                        {
                            throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される変数のスクリプト「{0}」は読み込めませんでした。テクニック\"{1}\"は存在しません。(スペルミス?)", text2, array2[1]));
                        }
                        SortedTechnique.Add(techniqueByName);
                    }
                    else
                    {
                        string[] array2 = text2.Split(new char[]
                        {
                            '?'
                        });
                        if (array2.Length == 2)
                        {
                            string[] array3 = array2[1].Split(new char[]
                            {
                                ':'
                            });
                            string[] array4 = array3;
                            for (int j = 0; j < array4.Length; j++)
                            {
                                string text3 = array4[j];
                                EffectTechnique techniqueByName2 = effect.GetTechniqueByName(text3);
                                if (techniqueByName2 == null)
                                {
                                    throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される変数のスクリプト「{0}」は読み込めませんでした。テクニック\"{1}\"は見つかりません。(スペルミス?)", text2, text3));
                                }
                                SortedTechnique.Add(techniqueByName2);
                            }
                        }
                        else if (array2.Length > 2)
                        {
                            throw new InvalidMMEEffectShaderException(string.Format("STANDARDGLOBALセマンティクスの指定される変数のスクリプト「{0}」は読み込めませんでした。\"?\"の数が多すぎます。", text2));
                        }
                    }
                    if (array.Length > 2)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("STANDARDGLOBALセマンティクスの指定される変数のスクリプト「{0}」では、複数回Techniqueの代入が行われていますが、最後の代入以外は無視されます。", StandardGlobalScript));
                    }
                }
            }
        }
    }
}