using MMF.Bone;
using MMF.Model;
using MMF.Model.PMX;
using MMF.Morph;
using MMF.Utility;
using SlimDX;
using SlimDX.Direct3D11;
using System.Linq;

namespace MMF.MME.VariableSubscriber.ControlInfoSubscriber
{
    internal sealed class ControlObjectSubscriber : SubscriberBase
    {
        private VariableType variableType;

        private string itemName;

        private string name;

        private TargetObject target;

        private bool isSelf;

        private static TargetObject[] NeedFloat = new TargetObject[]
        {
            TargetObject.X,
            TargetObject.Y,
            TargetObject.Z,
            TargetObject.Rx,
            TargetObject.Ry,
            TargetObject.Rz,
            TargetObject.Si,
            TargetObject.Tr,
            TargetObject.FaceName
        };

        private static TargetObject[] NeedFloat3 = new TargetObject[]
        {
            TargetObject.XYZ,
            TargetObject.Rxyz
        };

        public override string Semantics
        {
            get
            {
                return "CONTROLOBJECT";
            }
        }

        public override VariableType[] Types
        {
            get
            {
                VariableType[] array = new VariableType[5];
                array[0] = VariableType.Bool;
                array[1] = VariableType.Float;
                array[2] = VariableType.Float3;
                array[3] = VariableType.Float4;
                return array;
            }
        }

        internal ControlObjectSubscriber()
        {
        }

        private ControlObjectSubscriber(VariableType type, string itemName, string name, TargetObject target, bool isSelf)
        {
            variableType = type;
            this.itemName = itemName;
            this.name = name;
            this.target = target;
            this.isSelf = isSelf;
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            VariableType variableType = VariableType.Float4x4;
            TargetObject value = TargetObject.UnUsed;
            string text = null;
            string text2 = variable.GetVariableType().Description.TypeName.ToLower();
            string text3 = text2;
            if (text3 != null)
            {
                if (!(text3 == "bool"))
                {
                    if (!(text3 == "float"))
                    {
                        if (!(text3 == "float3"))
                        {
                            if (!(text3 == "float4"))
                            {
                                if (text3 == "float4x4")
                                {
                                    variableType = VariableType.Float4x4;
                                }
                            }
                            else
                            {
                                variableType = VariableType.Float4;
                            }
                        }
                        else
                        {
                            variableType = VariableType.Float3;
                        }
                    }
                    else
                    {
                        variableType = VariableType.Float;
                    }
                }
                else
                {
                    variableType = VariableType.Bool;
                }
            }
            EffectVariable annotation = EffectParseHelper.getAnnotation(variable, "name", "string");
            if (annotation == null)
            {
                throw new InvalidMMEEffectShaderException(string.Format("定義済みセマンティクス「CONTROLOBJECT」の適用されている変数「{0} {1}:CONTROLOBJECT」に対してはアノテーション「string name」は必須ですが、指定されませんでした。", text2, variable.Description.Name));
            }
            string @string = annotation.AsString().GetString();
            if (@string.ToLower().Equals("(self)"))
            {
                isSelf = true;
            }
            EffectVariable annotation2 = EffectParseHelper.getAnnotation(variable, "item", "string");
            if (annotation2 != null)
            {
                text = annotation2.AsString().GetString();
                text3 = text.ToLower();
                switch (text3)
                {
                    case "x":
                        value = TargetObject.X;
                        goto WHENISYOURBIRTHDAY;
                    case "y":
                        value = TargetObject.Y;
                        goto WHENISYOURBIRTHDAY;
                    case "z":
                        value = TargetObject.Z;
                        goto WHENISYOURBIRTHDAY;
                    case "xyz":
                        value = TargetObject.XYZ;
                        goto WHENISYOURBIRTHDAY;
                    case "rx":
                        value = TargetObject.Rx;
                        goto WHENISYOURBIRTHDAY;
                    case "ry":
                        value = TargetObject.Ry;
                        goto WHENISYOURBIRTHDAY;
                    case "rz":
                        value = TargetObject.Rz;
                        goto WHENISYOURBIRTHDAY;
                    case "rxyz":
                        value = TargetObject.Rxyz;
                        goto WHENISYOURBIRTHDAY;
                    case "si":
                        value = TargetObject.Si;
                        goto WHENISYOURBIRTHDAY;
                    case "tr":
                        value = TargetObject.Tr;
                        goto WHENISYOURBIRTHDAY;
                }
                value = ((variableType == VariableType.Float) ? TargetObject.FaceName : TargetObject.BoneName);
                WHENISYOURBIRTHDAY:
                if (ControlObjectSubscriber.NeedFloat.Contains(value) && variableType != VariableType.Float)
                {
                    throw new InvalidMMEEffectShaderException(string.Format("定義済みセマンティクス「CONTROLOBJECT」の適用されている変数「{0} {1}:CONTROLOBJECT」にはアノテーション「string item=\"{2}\"」が適用されていますが、「{2}」の場合は「float {1}:CONTROLOBJECT」である必要があります。", text2, variable.Description.Name, text));
                }
                if (ControlObjectSubscriber.NeedFloat3.Contains(value) && variableType != VariableType.Float3)
                {
                    throw new InvalidMMEEffectShaderException(string.Format("定義済みセマンティクス「CONTROLOBJECT」の適用されている変数「{0} {1}:CONTROLOBJECT」にはアノテーション「string item=\"{2}\"」が適用されていますが、「{2}」の場合は「float3 {1}:CONTROLOBJECT」である必要があります。", text2, variable.Description.Name, text));
                }
            }
            return new ControlObjectSubscriber(variableType, text, @string, value, isSelf);
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            System.Collections.Generic.IEnumerable<IDrawable> enumerable = from drawable in variable.Context.CurrentTargetContext.WorldSpace.DrawableResources
                                                                           where drawable.FileName == name
                                                                           select drawable;
            if (enumerable.Count<IDrawable>() != 0)
            {
                System.Collections.Generic.IEnumerator<IDrawable> enumerator = enumerable.GetEnumerator();
                enumerator.MoveNext();
                IDrawable drawable2 = isSelf ? variable.Model : enumerator.Current;
                if (target == TargetObject.UnUsed)
                {
                    switch (variableType)
                    {
                        case VariableType.Float4x4:
                            subscribeTo.AsMatrix().SetMatrix(variable.Context.MatrixManager.WorldMatrixManager.getWorldMatrix(drawable2));
                            break;
                        case VariableType.Float3:
                            subscribeTo.AsVector().Set(drawable2.Transformer.Position);
                            break;
                        case VariableType.Float4:
                            subscribeTo.AsVector().Set(new Vector4(drawable2.Transformer.Position, 1f));
                            break;
                        case VariableType.Float:
                            subscribeTo.AsScalar().Set(drawable2.Transformer.Scale.Length());
                            break;
                        case VariableType.Bool:
                            subscribeTo.AsScalar().Set(drawable2.Visibility);
                            break;
                    }
                }
                else if (target == TargetObject.BoneName)
                {
                    System.Collections.Generic.IEnumerable<PMXBone> enumerable2 = from bone in ((PMXModel)drawable2).Skinning.Bone
                                                                                  where bone.BoneName == itemName
                                                                                  select bone;
                    using (System.Collections.Generic.IEnumerator<PMXBone> enumerator2 = enumerable2.GetEnumerator())
                    {
                        if (enumerator2.MoveNext())
                        {
                            PMXBone current = enumerator2.Current;
                            Matrix matrix = current.GlobalPose * variable.Context.MatrixManager.WorldMatrixManager.getWorldMatrix(drawable2);
                            switch (variableType)
                            {
                                case VariableType.Float4x4:
                                    subscribeTo.AsMatrix().SetMatrix(matrix);
                                    break;
                                case VariableType.Float3:
                                    subscribeTo.AsVector().Set(Vector3.TransformCoordinate(current.Position, matrix));
                                    break;
                                case VariableType.Float4:
                                    subscribeTo.AsVector().Set(new Vector4(Vector3.TransformCoordinate(current.Position, matrix), 1f));
                                    break;
                            }
                        }
                    }
                }
                else if (target == TargetObject.FaceName)
                {
                    IMorphManager morphmanager = ((PMXModel)drawable2).Morphmanager;
                    subscribeTo.AsScalar().Set(morphmanager.getMorphProgress(name));
                }
                else
                {
                    switch (target)
                    {
                        case TargetObject.X:
                            subscribeTo.AsScalar().Set(drawable2.Transformer.Position.X);
                            break;
                        case TargetObject.Y:
                            subscribeTo.AsScalar().Set(drawable2.Transformer.Position.Y);
                            break;
                        case TargetObject.Z:
                            subscribeTo.AsScalar().Set(drawable2.Transformer.Position.Z);
                            break;
                        case TargetObject.XYZ:
                            subscribeTo.AsVector().Set(drawable2.Transformer.Position);
                            break;
                        case TargetObject.Rx:
                        case TargetObject.Ry:
                        case TargetObject.Rz:
                        case TargetObject.Rxyz:
                            {
                                float num;
                                float num2;
                                float num3;
                                if (!CGHelper.FactoringQuaternionXYZ(drawable2.Transformer.Rotation, out num, out num2, out num3))
                                {
                                    if (!CGHelper.FactoringQuaternionYZX(drawable2.Transformer.Rotation, out num2, out num3, out num))
                                    {
                                        CGHelper.FactoringQuaternionZXY(drawable2.Transformer.Rotation, out num3, out num, out num2);
                                    }
                                }
                                if (target == TargetObject.Rx)
                                {
                                    subscribeTo.AsScalar().Set(num);
                                }
                                else if (target == TargetObject.Ry)
                                {
                                    subscribeTo.AsScalar().Set(num2);
                                }
                                else if (target == TargetObject.Rz)
                                {
                                    subscribeTo.AsScalar().Set(num3);
                                }
                                else
                                {
                                    subscribeTo.AsVector().Set(new Vector3(num, num2, num3));
                                }
                                break;
                            }
                        case TargetObject.Si:
                            subscribeTo.AsScalar().Set(drawable2.Transformer.Scale.Length());
                            break;
                        case TargetObject.Tr:
                            break;
                        default:
                            throw new System.ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}