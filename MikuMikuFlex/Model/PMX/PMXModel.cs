using MMDFileParser.PMXModelParser;
using MMF.Bone;
using MMF.MME;
using MMF.Morph;
using MMF.Motion;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System.Diagnostics;

namespace MMF.Model.PMX
{
    public class PMXModel : ISubsetDivided, IMovable, IEdgeDrawable, IGroundShadowDrawable, IDrawable, System.IDisposable
    {
        private Vector4 groundShadowColor;

        private Vector4 selfShadowColor;

        public ISubresourceLoader SubresourceLoader
        {
            get;
            private set;
        }

        public ModelData Model
        {
            get;
            private set;
        }

        public IBufferManager BufferManager
        {
            get;
            private set;
        }

        public ISubsetManager SubsetManager
        {
            get;
            private set;
        }

        public IMorphManager Morphmanager
        {
            get;
            private set;
        }

        public MMEEffectManager Effect
        {
            get;
            private set;
        }

        public IToonTextureManager ToonManager
        {
            get;
            private set;
        }

        public RenderContext RenderContext
        {
            get;
            private set;
        }

        private EffectPass ZPlotPass
        {
            get;
            set;
        }

        private bool IsInitialized
        {
            get;
            set;
        }

        public bool Visibility
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public int SubsetCount
        {
            get
            {
                return SubsetManager.SubsetCount;
            }
        }

        public int VertexCount
        {
            get
            {
                return BufferManager.VerticiesCount;
            }
        }

        public Vector4 SelfShadowColor
        {
            get
            {
                return selfShadowColor;
            }
            set
            {
                selfShadowColor = value;
            }
        }

        public Vector4 GroundShadowColor
        {
            get
            {
                return groundShadowColor;
            }
            set
            {
                groundShadowColor = value;
            }
        }

        public ITransformer Transformer
        {
            get;
            private set;
        }

        public IMotionManager MotionManager
        {
            get;
            private set;
        }

        public ISkinningProvider Skinning
        {
            get;
            private set;
        }

        public PMXModel(ModelData modeldata, ISubresourceLoader subResourceLoader, string filename)
        {
            Model = modeldata;
            SubresourceLoader = subResourceLoader;
            Transformer = new BasicTransformer();
            SubsetManager = new PMXSubsetManager(this, modeldata);
            ToonManager = new PMXToonTextureManager();
            SelfShadowColor = new Vector4(0f, 0f, 0f, 1f);
            GroundShadowColor = new Vector4(0f, 0f, 0f, 1f);
            FileName = filename;
            Visibility = true;
        }

        private PMXModel()
        {
        }

        public void LoadFromEffectFile(string filePath)
        {
            Effect = MMEEffectManager.Load(filePath, this, RenderContext, SubresourceLoader);
        }

        public virtual void Draw()
        {
            Effect.ApplyAllMatrixVariables();
            Skinning.ApplyEffect(Effect.EffectFile);
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(BufferManager.VertexBuffer, BasicInputLayout.SizeInBytes, 0));
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.SetIndexBuffer(BufferManager.IndexBuffer, Format.R32_UInt, 0);
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.InputLayout = BufferManager.VertexLayout;
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            SubsetManager.DrawAll();
        }

        public void DrawEdge()
        {
            Effect.ApplyAllMatrixVariables();
            Skinning.ApplyEffect(Effect.EffectFile);
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(BufferManager.VertexBuffer, BasicInputLayout.SizeInBytes, 0));
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.SetIndexBuffer(BufferManager.IndexBuffer, Format.R32_UInt, 0);
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.InputLayout = BufferManager.VertexLayout;
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            SubsetManager.DrawEdges();
        }

        public void DrawGroundShadow()
        {
            Effect.ApplyAllMatrixVariables();
            Skinning.ApplyEffect(Effect.EffectFile);
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(BufferManager.VertexBuffer, BasicInputLayout.SizeInBytes, 0));
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.SetIndexBuffer(BufferManager.IndexBuffer, Format.R32_UInt, 0);
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.InputLayout = BufferManager.VertexLayout;
            RenderContext.DeviceManager.Device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            SubsetManager.DrawGroundShadow();
        }

        public void Update()
        {
            if (BufferManager != null)
            {
                BufferManager.RecreateVerticies();
            }
            Morphmanager.UpdateFrame();
            Skinning.UpdateSkinning(Morphmanager);
            foreach (ISubset current in SubsetManager.Subsets)
            {
                current.MaterialInfo.UpdateMaterials();
            }
        }

        public virtual void ApplyMove()
        {
        }

        public void Load(RenderContext renderContext)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            RenderContext = renderContext;
            SlimDX.Direct3D11.Device device = RenderContext.DeviceManager.Device;
            if (!IsInitialized)
            {
                ToonManager.Initialize(RenderContext, SubresourceLoader);
                SubsetManager.Initialze(RenderContext, Effect, SubresourceLoader, ToonManager);
                Effect = InitializeEffect();
                SubsetManager.ResetEffect(Effect);
                ZPlotPass = Effect.EffectFile.GetTechniqueByIndex(1).GetPassByIndex(0);
                InitializeBuffers(device);
                Skinning = InitializeSkinning();
                Morphmanager = new PMXMorphManager(this);
                IsInitialized = true;
            }
            MotionManager = InitializeMotionManager();
            InitializeOther(device);
            stopwatch.Stop();
            Trace.WriteLine(stopwatch.ElapsedMilliseconds + "ms");
        }

        public void LoadEffect(string effectFile)
        {
            if (Effect != null)
            {
                Effect.Dispose();
            }
            if (string.IsNullOrEmpty(effectFile))
            {
                Effect = MMEEffectManager.Load("Shader\\DefaultShader.fx", this, RenderContext, SubresourceLoader);
            }
            else
            {
                Effect = MMEEffectManager.Load(effectFile, this, RenderContext, SubresourceLoader);
            }
            SubsetManager.ResetEffect(Effect);
        }

        protected virtual ISkinningProvider InitializeSkinning()
        {
            return new PMXSkeleton(Model);
        }

        protected virtual IMotionManager InitializeMotionManager()
        {
            BasicMotionManager basicMotionManager = new BasicMotionManager(RenderContext);
            basicMotionManager.Initialize(Model, Morphmanager, Skinning, BufferManager);
            ((PMXSkeleton)Skinning).KinematicsProviders.Insert(0, basicMotionManager);
            return basicMotionManager;
        }

        protected virtual MMEEffectManager InitializeEffect()
        {
            return MMEEffectManager.LoadFromResource("MMF.Resource.Shader.DefaultShader.fx", this, RenderContext, SubresourceLoader);
        }

        protected virtual void InitializeOther(SlimDX.Direct3D11.Device device)
        {
        }

        protected virtual void InitializeBuffers(SlimDX.Direct3D11.Device device)
        {
            BufferManager = new PMXModelBufferManager();
            BufferManager.Initialize(Model, device, Effect.EffectFile);
        }

        public virtual void Dispose()
        {
            if (Skinning != null)
            {
                Skinning.Dispose();
                Skinning = null;
            }

            if (Effect != null)
            {
                Effect.Dispose();
                Effect = null;
            }

            if (BufferManager != null)
            {
                BufferManager.Dispose();
                BufferManager = null;
            }

            if (ToonManager != null)
            {
                ToonManager.Dispose();
                ToonManager = null;
            }

            if (SubsetManager != null)
            {
                SubsetManager.Dispose();
                SubsetManager = null;
            }
        }

        public static PMXModel FromFile(string filePath)
        {
            string directoryName = System.IO.Path.GetDirectoryName(filePath);
            return PMXModel.FromFile(filePath, directoryName);
        }

        public static PMXModel FromFile(string filePath, string textureFolder)
        {
            return PMXModel.FromFile(filePath, new BasicSubresourceLoader(textureFolder));
        }

        public static PMXModel FromFile(string filePath, ISubresourceLoader loader)
        {
            PMXModel result;
            using (System.IO.FileStream fileStream = System.IO.File.OpenRead(filePath))
            {
                result = new PMXModel(ModelData.GetModel(fileStream), loader, System.IO.Path.GetFileName(filePath));
            }
            return result;
        }

        public static PMXModel OpenLoad(string filePath, RenderContext context)
        {
            PMXModel pMXModel = PMXModel.FromFile(filePath);
            pMXModel.Load(context);
            return pMXModel;
        }

        public static PMXModel OpenLoad(string filePath, string textureFolder, RenderContext context)
        {
            PMXModel pMXModel = PMXModel.FromFile(filePath, textureFolder);
            pMXModel.Load(context);
            return pMXModel;
        }

        public static PMXModel OpenLoad(string filePath, ISubresourceLoader loader, RenderContext context)
        {
            PMXModel pMXModel = PMXModel.FromFile(filePath, loader);
            pMXModel.Load(context);
            return pMXModel;
        }
    }
}
