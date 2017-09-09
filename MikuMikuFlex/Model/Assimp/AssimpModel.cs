using Assimp;
using MMF.MME;
using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.Model.Assimp
{
    public class AssimpModel : IDrawable, System.IDisposable, ISubsetDivided
    {
        private Scene modelScene;

        private ISubresourceLoader loader;

        private RenderContext context;

        private System.Collections.Generic.List<ISubset> subsets = new System.Collections.Generic.List<ISubset>();

        private MMEEffectManager effectManager;

        private InputLayout layout;

        public bool Visibility
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            private set;
        }

        public int SubsetCount
        {
            get
            {
                return subsets.Count;
            }
        }

        public int VertexCount
        {
            get;
            private set;
        }

        public ITransformer Transformer
        {
            get;
            private set;
        }

        public Vector4 SelfShadowColor
        {
            get;
            set;
        }

        public Vector4 GroundShadowColor
        {
            get;
            set;
        }

        public AssimpModel(RenderContext context, string fileName)
        {
            FileName = System.IO.Path.GetFileName(fileName);
            this.context = context;
            loader = new BasicSubresourceLoader(System.IO.Path.GetDirectoryName(fileName));
            AssimpImporter assimpImporter = new AssimpImporter();
            modelScene = assimpImporter.ImportFile(fileName, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals);
            Visibility = true;
            Initialize();
        }

        public AssimpModel(RenderContext context, string fileName, Scene modelScene, ISubresourceLoader loader)
        {
            this.modelScene = modelScene;
            this.context = context;
            FileName = fileName;
            this.loader = loader;
            Visibility = true;
            Initialize();
        }

        private void Initialize()
        {
            Transformer = new BasicTransformer();
            for (int i = 0; i < modelScene.Meshes.Length; i++)
            {
                subsets.Add(new AssimpSubset(context, loader, this, modelScene, i));
            }
            effectManager = MMEEffectManager.LoadFromResource("MMF.Resource.Shader.DefaultShader.fx", this, context, loader);
            layout = new InputLayout(context.DeviceManager.Device, effectManager.EffectFile.GetTechniqueByIndex(1).GetPassByIndex(0).Description.Signature, BasicInputLayout.VertexElements);
        }

        public void Dispose()
        {
            if (layout != null && !layout.Disposed)
            {
                layout.Dispose();
            }
            effectManager.Dispose();
            foreach (ISubset current in subsets)
            {
                current.Dispose();
            }
        }

        public void Draw()
        {
            effectManager.ApplyAllMatrixVariables();
            context.DeviceManager.Device.ImmediateContext.InputAssembler.InputLayout = layout;
            context.DeviceManager.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            effectManager.EffectFile.GetVariableBySemantic("BONETRANS").AsMatrix().SetMatrixArray(new Matrix[]
            {
                Matrix.Identity
            });
            foreach (ISubset current in subsets)
            {
                effectManager.ApplyAllMaterialVariables(current.MaterialInfo);
                effectManager.ApplyEffectPass(current, MMEEffectPassType.Object, delegate (ISubset isubset)
                {
                    isubset.Draw(context.DeviceManager.Device);
                });
            }
        }

        public void Update()
        {
        }
    }
}
