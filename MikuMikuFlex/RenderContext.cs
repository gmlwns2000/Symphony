using MMF.DeviceManager;
using MMF.Light;
using MMF.Matricies;
using MMF.Matricies.Camera;
using MMF.Matricies.Projection;
using MMF.Matricies.World;
using MMF.Model;
using MMF.Motion;
using SlimDX;
using SlimDX.Direct2D;
using SlimDX.Direct3D11;
using System.Windows.Forms;

namespace MMF
{
    public class RenderContext : System.IDisposable
    {
        private System.Collections.Generic.Dictionary<Control, ScreenContext> screenContexts = new System.Collections.Generic.Dictionary<Control, ScreenContext>();

        public System.Collections.Generic.List<WorldSpace> UpdateRequireWorlds = new System.Collections.Generic.List<WorldSpace>();

        public LightMatrixManager LightManager;

        private bool disposeDeviceManager = false;

        public MotionTimer Timer;

        internal System.Collections.Generic.List<System.IDisposable> Disposables = new System.Collections.Generic.List<System.IDisposable>();

        public RenderTargetView[] CurrentRenderColorTargets = new RenderTargetView[8];

        public DepthStencilView CurrentRenderDepthStencilTarget;

        public PanelObserver CurrentPanelObserver;

        public event System.EventHandler Update = delegate (object param0, System.EventArgs param1)
        {
        };

        public ITargetContext CurrentTargetContext
        {
            get;
            private set;
        }

        public IDeviceManager DeviceManager
        {
            get;
            private set;
        }

        public MatrixManager MatrixManager
        {
            get
            {
                return CurrentTargetContext.MatrixManager;
            }
        }

        public Color4 CurrentClearColor
        {
            get;
            set;
        }

        public float CurrentClearDepth
        {
            get;
            set;
        }

        public RasterizerState CullingRasterizerState
        {
            get;
            private set;
        }

        public RasterizerState NonCullingRasterizerState
        {
            get;
            private set;
        }

        public Factory D2DFactory
        {
            get;
            set;
        }

        public SlimDX.DirectWrite.Factory DWFactory
        {
            get;
            private set;
        }

        public System.Collections.Generic.Dictionary<Control, ScreenContext> ScreenContexts
        {
            get
            {
                return screenContexts;
            }
        }

        public static RenderContext CreateContext(IDeviceManager deviceManager = null)
        {
            RenderContext renderContext = new RenderContext
            {
                DeviceManager = deviceManager
            };
            renderContext.Initialize();
            return renderContext;
        }

        public RenderContext()
        {
        }

        public RenderContext(BasicGraphicDeviceManager deviceManager)
        {
            DeviceManager = deviceManager;
        }

        public void Dispose()
        {
            foreach (System.Collections.Generic.KeyValuePair<Control, ScreenContext> current in screenContexts)
            {
                current.Value.Dispose();
            }
            if (CullingRasterizerState != null && !CullingRasterizerState.Disposed)
            {
                CullingRasterizerState.Dispose();
            }
            if (NonCullingRasterizerState != null && !NonCullingRasterizerState.Disposed)
            {
                NonCullingRasterizerState.Dispose();
            }
            foreach (System.IDisposable current2 in Disposables)
            {
                current2.Dispose();
            }
            if (D2DFactory != null && !D2DFactory.Disposed)
            {
                D2DFactory.Dispose();
            }
            if (DWFactory != null && !DWFactory.Disposed)
            {
                DWFactory.Dispose();
            }
            if (disposeDeviceManager)
            {
                DeviceManager.Dispose();
            }
        }

        public void Initialize()
        {
            InitializeDevices();
            Timer = new MotionTimer(this);
        }

        public ScreenContext Initialize(Control targetControl)
        {
            InitializeDevices();
            Timer = new MotionTimer(this);
            MatrixManager manager = InitializeMatricies();
            ScreenContext screenContext = new ScreenContext(targetControl, this, manager);
            screenContexts.Add(targetControl, screenContext);
            CurrentTargetContext = screenContext;
            ResetTargets();
            return screenContext;
        }

        private MatrixManager InitializeMatricies()
        {
            BasicCamera cam = new BasicCamera(new Vector3(0f, 20f, -40f), new Vector3(0f, 3f, 0f), new Vector3(0f, 1f, 0f));
            BasicProjectionMatrixProvider basicProjectionMatrixProvider = new BasicProjectionMatrixProvider();
            basicProjectionMatrixProvider.InitializeProjection(0.7853982f, 1.618f, 1f, 2000f);
            MatrixManager matrixManager = new MatrixManager(new BasicWorldMatrixProvider(), cam, basicProjectionMatrixProvider);
            LightManager = new LightMatrixManager(matrixManager);
            return matrixManager;
        }

        private void InitializeDevices()
        {
            InitializeMatricies();
            if (DeviceManager == null)
            {
                disposeDeviceManager = true;
                DeviceManager = new BasicGraphicDeviceManager();
                DeviceManager.Load();
            }
            RasterizerStateDescription description = default(RasterizerStateDescription);
            description.CullMode = CullMode.Back;
            description.FillMode = SlimDX.Direct3D11.FillMode.Solid;
            CullingRasterizerState = RasterizerState.FromDescription(DeviceManager.Device, description);
            description.CullMode = CullMode.None;
            NonCullingRasterizerState = RasterizerState.FromDescription(DeviceManager.Device, description);
            DWFactory = new SlimDX.DirectWrite.Factory(SlimDX.DirectWrite.FactoryType.Isolated);
            D2DFactory = new Factory(SlimDX.Direct2D.FactoryType.Multithreaded, DebugLevel.Information);
        }

        private void ResetTargets()
        {
            CurrentRenderColorTargets[0] = CurrentTargetContext.RenderTargetView;
            CurrentRenderDepthStencilTarget = CurrentTargetContext.DepthTargetView;
            DeviceManager.Context.OutputMerger.SetTargets(CurrentRenderDepthStencilTarget, CurrentRenderColorTargets);
        }

        public void ClearScreenTarget(Color4 color)
        {
            ResetTargets();
            DeviceManager.Context.ClearRenderTargetView(CurrentTargetContext.RenderTargetView, color);
            DeviceManager.Context.ClearDepthStencilView(CurrentTargetContext.DepthTargetView, DepthStencilClearFlags.Depth, 1f, 0);
        }

        public void UpdateWorlds()
        {
            foreach (WorldSpace current in UpdateRequireWorlds)
            {
                current.UpdateAllDynamicTexture();
                foreach (IDrawable current2 in current.DrawableResources)
                {
                    current2.Update();
                }
            }
            Update(this, new System.EventArgs());
        }

        public void SetRenderScreen(ITargetContext context)
        {
            CurrentTargetContext = context;
            context.SetViewport();
        }

        public ScreenContext CreateScreenContext(Control control)
        {
            BasicCamera cam = new BasicCamera(new Vector3(0f, 20f, -40f), new Vector3(0f, 3f, 0f), new Vector3(0f, 1f, 0f));
            BasicProjectionMatrixProvider basicProjectionMatrixProvider = new BasicProjectionMatrixProvider();
            basicProjectionMatrixProvider.InitializeProjection(0.7853982f, 1.618f, 1f, 200f);
            MatrixManager manager = new MatrixManager(new BasicWorldMatrixProvider(), cam, basicProjectionMatrixProvider);
            ScreenContext screenContext = new ScreenContext(control, this, manager);
            screenContexts.Add(control, screenContext);
            return screenContext;
        }
    }
}
