using MMDFileParser.PMXModelParser;
using MMF.DeviceManager;
using MMF.MME.Includer;
using MMF.MME.VariableSubscriber;
using MMF.MME.VariableSubscriber.ConstantSubscriber;
using MMF.MME.VariableSubscriber.ControlInfoSubscriber;
using MMF.MME.VariableSubscriber.MaterialSubscriber;
using MMF.MME.VariableSubscriber.MatrixSubscriber;
using MMF.MME.VariableSubscriber.MouseSubscriber;
using MMF.MME.VariableSubscriber.PeculiarValueSubscriber;
using MMF.MME.VariableSubscriber.ScreenInfoSubscriber;
using MMF.MME.VariableSubscriber.TextureSubscriber;
using MMF.MME.VariableSubscriber.TimeSubscriber;
using MMF.MME.VariableSubscriber.WorldInfoSubscriber;
using MMF.Model;
using MMF.Utility;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using System.Linq;
using System.Text.RegularExpressions;

namespace MMF.MME
{
    public class MMEEffectManager : System.IDisposable
    {
        public const string MMFDefinition = "MMF";

        public const string MMFDefaultShaderResourcePath = "MMF.Resource.Shader.DefaultShader.fx";

        private static string applicationDefinition = "MMFApp";

        private readonly IDrawable model;

        private string fileName;

        public static EffectSubscriberDictionary EffectSubscriber
        {
            get;
            private set;
        }

        public static PeculiarEffectSubscriberDictionary PeculiarEffectSubscriber
        {
            get;
            private set;
        }

        public static System.Collections.Generic.List<ShaderMacro> EffectMacros
        {
            get;
            private set;
        }

        public static Include EffectInclude
        {
            get;
            private set;
        }

        public static string ApplicationDefinition
        {
            get
            {
                return MMEEffectManager.applicationDefinition;
            }
            set
            {
                if (MMEEffectManager.applicationDefinition != value)
                {
                    MMEEffectManager.EffectMacros.Remove(new ShaderMacro(MMEEffectManager.applicationDefinition));
                    MMEEffectManager.applicationDefinition = value;
                    MMEEffectManager.EffectMacros.Add(new ShaderMacro(value));
                }
            }
        }

        private RenderContext Context
        {
            get;
            set;
        }

        public MMEEffectInfo EffectInfo
        {
            get;
            private set;
        }

        public System.Collections.Generic.Dictionary<EffectVariable, SubscriberBase> ActiveSubscriberByModel
        {
            get;
            private set;
        }

        public System.Collections.Generic.Dictionary<EffectVariable, SubscriberBase> ActiveSubscriberByMaterial
        {
            get;
            private set;
        }

        public System.Collections.Generic.Dictionary<EffectVariable, PeculiarValueSubscriberBase> ActivePeculiarSubscriber
        {
            get;
            private set;
        }

        public System.Collections.Generic.Dictionary<string, RenderTargetView> RenderColorTargetViewes
        {
            get;
            private set;
        }

        public System.Collections.Generic.Dictionary<string, DepthStencilView> RenderDepthStencilTargets
        {
            get;
            private set;
        }

        public System.Collections.Generic.List<MMEEffectTechnique> Techniques
        {
            get;
            private set;
        }

        public ISubresourceLoader SubresourceLoader
        {
            get;
            private set;
        }

        private Effect effect
        {
            get;
            set;
        }

        public MMEEffectManager DefaultShader
        {
            get;
            private set;
        }

        public Effect EffectFile
        {
            get
            {
                return effect;
            }
            private set
            {
                effect = value;
            }
        }

        private MMEEffectManager(string fileName, RenderContext context, Effect effect, IDrawable model, ISubresourceLoader loader)
        {
            this.fileName = fileName;
            SubresourceLoader = loader;
            if (!fileName.Equals("MMF.Resource.Shader.DefaultShader.fx"))
            {
                DefaultShader = MMEEffectManager.LoadFromResource("MMF.Resource.Shader.DefaultShader.fx", model, context, new BasicSubresourceLoader("Shader"));
            }
            else
            {
                DefaultShader = this;
            }
            Context = context;
            EffectFile = effect;
            EffectInfo = new MMEEffectInfo(effect);
            ActiveSubscriberByMaterial = new System.Collections.Generic.Dictionary<EffectVariable, SubscriberBase>();
            ActiveSubscriberByModel = new System.Collections.Generic.Dictionary<EffectVariable, SubscriberBase>();
            ActivePeculiarSubscriber = new System.Collections.Generic.Dictionary<EffectVariable, PeculiarValueSubscriberBase>();
            Techniques = new System.Collections.Generic.List<MMEEffectTechnique>();
            RenderColorTargetViewes = new System.Collections.Generic.Dictionary<string, RenderTargetView>();
            RenderDepthStencilTargets = new System.Collections.Generic.Dictionary<string, DepthStencilView>();
            this.model = model;
            int num = effect.Description.GlobalVariableCount;
            for (int i = 0; i < num; i++)
            {
                string text = Regex.Replace(effect.GetVariableByIndex(i).Description.Semantic, "[0-9]", "");
                string text2 = Regex.Replace(effect.GetVariableByIndex(i).Description.Semantic, "[^0-9]", "");
                int semanticIndex = string.IsNullOrEmpty(text2) ? 0 : int.Parse(text2);
                string text3 = effect.GetVariableByIndex(i).GetVariableType().Description.TypeName.ToLower();
                text = text.ToUpper();
                if (MMEEffectManager.EffectSubscriber.ContainsKey(text))
                {
                    SubscriberBase subscriberBase = MMEEffectManager.EffectSubscriber[text];
                    EffectVariable variableByIndex = effect.GetVariableByIndex(i);
                    subscriberBase.CheckType(variableByIndex);
                    if (subscriberBase.UpdateTiming == UpdateBy.Material)
                    {
                        ActiveSubscriberByMaterial.Add(variableByIndex, subscriberBase.GetSubscriberInstance(variableByIndex, context, this, semanticIndex));
                    }
                    else
                    {
                        ActiveSubscriberByModel.Add(variableByIndex, subscriberBase.GetSubscriberInstance(variableByIndex, context, this, semanticIndex));
                    }
                }
                else if (text3.Equals("texture") || text3.Equals("texture2d") || text3.Equals("texture3d") || text3.Equals("texturecube"))
                {
                    SubscriberBase subscriberBase = new TextureSubscriber();
                    EffectVariable variableByIndex = effect.GetVariableByIndex(i);
                    subscriberBase.CheckType(variableByIndex);
                    ActiveSubscriberByModel.Add(variableByIndex, subscriberBase.GetSubscriberInstance(variableByIndex, context, this, semanticIndex));
                }
                else
                {
                    string text4 = effect.GetVariableByIndex(i).Description.Name;
                    text4 = text4.ToLower();
                    if (MMEEffectManager.PeculiarEffectSubscriber.ContainsKey(text4))
                    {
                        ActivePeculiarSubscriber.Add(effect.GetVariableByIndex(i), MMEEffectManager.PeculiarEffectSubscriber[text4]);
                    }
                }
            }
            num = effect.Description.ConstantBufferCount;
            for (int i = 0; i < num; i++)
            {
                string text4 = effect.GetConstantBufferByIndex(i).Description.Name;
                text4 = text4.ToUpper();
                if (MMEEffectManager.EffectSubscriber.ContainsKey(text4))
                {
                    SubscriberBase subscriberBase = MMEEffectManager.EffectSubscriber[text4];
                    EffectConstantBuffer constantBufferByIndex = effect.GetConstantBufferByIndex(i);
                    subscriberBase.CheckType(constantBufferByIndex);
                    if (subscriberBase.UpdateTiming == UpdateBy.Material)
                    {
                        ActiveSubscriberByMaterial.Add(constantBufferByIndex, subscriberBase.GetSubscriberInstance(constantBufferByIndex, context, this, 0));
                    }
                    else
                    {
                        ActiveSubscriberByModel.Add(constantBufferByIndex, subscriberBase.GetSubscriberInstance(constantBufferByIndex, context, this, 0));
                    }
                }
            }
            int subsetCount = (model is ISubsetDivided) ? ((ISubsetDivided)model).SubsetCount : 1;
            foreach (EffectTechnique current in EffectInfo.SortedTechnique)
            {
                Techniques.Add(new MMEEffectTechnique(this, current, subsetCount, context));
            }
        }

        public void Dispose()
        {
            EffectFile.Dispose();
            foreach (System.Collections.Generic.KeyValuePair<EffectVariable, SubscriberBase> current in ActiveSubscriberByMaterial)
            {
                if (current.Value is System.IDisposable)
                {
                    System.IDisposable disposable = (System.IDisposable)current.Value;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
            foreach (System.Collections.Generic.KeyValuePair<EffectVariable, SubscriberBase> current2 in ActiveSubscriberByModel)
            {
                if (current2.Value is System.IDisposable)
                {
                    System.IDisposable disposable = (System.IDisposable)current2.Value;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        internal static void IniatializeMMEEffectManager(IDeviceManager deviceManager)
        {
            MMEEffectManager.EffectSubscriber = new EffectSubscriberDictionary
            {
                new WorldMatrixSubscriber(),
                new ProjectionMatrixSubscriber(),
                new ViewMatrixSubscriber(),
                new WorldInverseMatrixSubscriber(),
                new WorldTransposeMatrixSubscriber(),
                new WorldInverseTransposeMatrixSubscriber(),
                new ViewInverseMatrixSubscriber(),
                new ViewTransposeMatrixSubscriber(),
                new ViewInverseTransposeMatrixSubsriber(),
                new ProjectionInverseMatrixSubscriber(),
                new ProjectionTransposeMatrixSubscriber(),
                new ProjectionInverseTransposeMatrixSubscriber(),
                new WorldViewMatrixSubscriber(),
                new WorldViewInverseMatrixSubscriber(),
                new WorldViewTransposeMatrixSubscriber(),
                new ViewProjectionMatrixSubscriber(),
                new ViewProjectionInverseMatrixSubscriber(),
                new ViewProjectionTransposeMatrixSubscriber(),
                new ViewProjectionInverseTransposeMatrixSubscriber(),
                new WorldViewProjectionMatrixSubscriber(),
                new WorldViewProjectionInverseMatrixSubscriber(),
                new WorldViewProjectionTransposeMatrixSubscriber(),
                new WorldViewProjectionInverseTransposeMatrixSubscriber(),
                new DiffuseVectorSubscriber(),
                new AmbientVectorSubscriber(),
                new SpecularVectorSubscriber(),
                new SpecularPowerSubscriber(),
                new ToonVectorSubscriber(),
                new EdgeVectorSubscriber(),
                new GroundShadowColorVectorSubscriber(),
                new MaterialTextureSubscriber(),
                new MaterialSphereMapSubscriber(),
                new MaterialToonTextureSubscriber(),
                new AddingTextureSubscriber(),
                new MultiplyingTextureSubscriber(),
                new AddingSphereTextureSubscriber(),
                new MultiplyingSphereTextureSubscriber(),
                new EdgeThicknessSubscriber(),
                new PositionSubscriber(),
                new DirectionSubscriber(),
                new TimeSubScriber(),
                new ElapsedTimeSubScriber(),
                new MousePositionSubscriber(),
                new LeftMouseDownSubscriber(),
                new MiddleMouseDownSubscriber(),
                new RightMouseDownSubscriber(),
                new ViewPortPixelSizeScriber(),
                new BasicMaterialConstantSubscriber(),
                new FullMaterialConstantSubscriber(),
                new ControlObjectSubscriber(),
                new RenderColorTargetSubscriber(),
                new RenderDepthStencilTargetSubscriber()
            };
            MMEEffectManager.PeculiarEffectSubscriber = new PeculiarEffectSubscriberDictionary
            {
                new OpAddSubscriber(),
                new ParthfSubscriber(),
                new SpAddSubscriber(),
                new SubsetCountSubscriber(),
                new TranspSubscriber(),
                new Use_SpheremapSubscriber(),
                new Use_TextureSubscriber(),
                new Use_ToonSubscriber(),
                new VertexCountSubscriber()
            };
            MMEEffectManager.EffectMacros = new System.Collections.Generic.List<ShaderMacro>();
            MMEEffectManager.EffectMacros.Add(new ShaderMacro("MMF"));
            MMEEffectManager.EffectMacros.Add(new ShaderMacro(MMEEffectManager.ApplicationDefinition));
            MMEEffectManager.EffectMacros.Add(new ShaderMacro("MMM_LightCount", "3"));
            if (deviceManager.DeviceFeatureLevel == FeatureLevel.Level_11_0)
            {
                MMEEffectManager.EffectMacros.Add(new ShaderMacro("DX_LEVEL_11_0"));
            }
            else
            {
                MMEEffectManager.EffectMacros.Add(new ShaderMacro("DX_LEVEL_10_1"));
            }
            MMEEffectManager.EffectInclude = new BasicEffectIncluder();
        }

        public void ApplyAllMatrixVariables()
        {
            SubscribeArgument variable = new SubscribeArgument(model, Context);
            foreach (System.Collections.Generic.KeyValuePair<EffectVariable, SubscriberBase> current in ActiveSubscriberByModel)
            {
                current.Value.Subscribe(current.Key, variable);
            }
        }

        public void ApplyAllMaterialVariables(MaterialInfo info)
        {
            SubscribeArgument subscribeArgument = new SubscribeArgument(info, model, Context);
            foreach (System.Collections.Generic.KeyValuePair<EffectVariable, SubscriberBase> current in ActiveSubscriberByMaterial)
            {
                current.Value.Subscribe(current.Key, subscribeArgument);
            }
            foreach (System.Collections.Generic.KeyValuePair<EffectVariable, PeculiarValueSubscriberBase> current2 in ActivePeculiarSubscriber)
            {
                current2.Value.Subscribe(current2.Key, subscribeArgument);
            }
        }

        public void ApplyEffectPass(ISubset ipmxSubset, MMEEffectPassType passType, System.Action<ISubset> drawAction)
        {
            if (ipmxSubset.MaterialInfo.DiffuseColor.W != 0f)
            {
                if (ipmxSubset.DoCulling)
                {
                    Context.DeviceManager.Context.Rasterizer.State = Context.CullingRasterizerState;
                }
                else
                {
                    Context.DeviceManager.Context.Rasterizer.State = Context.NonCullingRasterizerState;
                }
                MMEEffectTechnique[] array = (from teq in Techniques
                                              where teq.Subset.Contains(ipmxSubset.SubsetId) && teq.MMDPassAnnotation == passType && MMEEffectTechnique.CheckExtebdedBoolean(teq.UseToon, ipmxSubset.MaterialInfo.IsToonUsed) && MMEEffectTechnique.CheckExtebdedBoolean(teq.UseTexture, ipmxSubset.MaterialInfo.MaterialTexture != null) && MMEEffectTechnique.CheckExtebdedBoolean(teq.UseSphereMap, ipmxSubset.MaterialInfo.MaterialSphereMap != null) && MMEEffectTechnique.CheckExtebdedBoolean(teq.MulSphere, ipmxSubset.MaterialInfo.SphereMode == SphereMode.Multiply)
                                              select teq).ToArray<MMEEffectTechnique>();
                MMEEffectTechnique[] array2 = array;
                int num = 0;
                if (num < array2.Length)
                {
                    MMEEffectTechnique mMEEffectTechnique = array2[num];
                    mMEEffectTechnique.ExecuteTechnique(Context.DeviceManager.Context, drawAction, ipmxSubset);
                }
            }
        }

        public static MMEEffectManager Load(string str, IDrawable model, RenderContext context, ISubresourceLoader loader)
        {
            return new MMEEffectManager(str, context, CGHelper.CreateEffectFx5(str, context.DeviceManager.Device), model, loader);
        }

        internal static MMEEffectManager LoadFromResource(string str, IDrawable model, RenderContext context, ISubresourceLoader loader)
        {
            Effect effect = CGHelper.CreateEffectFx5FromResource(str, context.DeviceManager.Device);
            return new MMEEffectManager(str, context, effect, model, loader);
        }
    }
}
