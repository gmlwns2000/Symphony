using MMF.Model.PMX;
using MMF.Motion;
using Symphony.Player;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Symphony.Dancer
{
    /// <summary>
    /// DanceLiteControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DanceLiteControl : UserControl, IDisposable
    {
        MainWindow mw;
        PlayerCore np;
        PlotLite pl;

        bool _ar = false;
        bool allowRender
        {
            get
            {
                return _ar;
            }
            set
            {
                _ar = value;

                RenderControl.AllowRendering = allowRender && !mw.isMoving && mw.isFrameUpadteAllowed;
            }
        }

        Thread Loader;
        public PMXModelWithPhysics model;
        public IMotionProvider motion;

        public DanceLiteControl(MainWindow mw, PlayerCore np, PlotLite pl)
        {
            InitializeComponent();

            this.pl = pl;
            this.np = np;
            this.mw = mw;

            mw.UpdateAllowChanged += Mw_UpdateAllowChanged;
            mw.MovingStateChanged += Mw_MovingStateChanged;

            RenderControl.FPS = 1000 / mw.GUIUpdate;
            RenderControl.AllowRenderingChanged += RenderControl_AllowRenderingChanged;
            allowRender = false;

            Logger.Log("DLR: " + pl.Metadata.ToString());

            Np_PlayStarted();

            np.PlayPaused += Np_PlayPaused;
            np.PlayResumed += Np_PlayResumed;
            np.PlaySeeked += Np_PlaySeeked;
        }

        bool preState = false;
        private void RenderControl_AllowRenderingChanged(object sender, EventArgs e)
        {
            if (mw != null)
            {
                if (preState != RenderControl.AllowRendering)
                {
                    Np_PlaySeeked();
                }
            }
            preState = RenderControl.AllowRendering;
        }

        public void Np_PlaySeeked()
        {
            if (motion != null && model != null)
            {
                motion.Stop();
                motion.Start((float)(np.GetPosition(TimeUnit.MilliSecond) / 1000) * RenderControl.RenderContext.Timer.MotionFramePerSecond, ActionAfterMotion.Nothing);

                if (np.isPlay && np.isPaused)
                {
                    motion.Stop();
                }
                else if (!np.isPlay)
                {
                    motion.Stop();
                }
            }
        }

        private void Np_PlayResumed()
        {
            if (motion != null && model != null)
            {
                motion.Start((float)(np.GetPosition(TimeUnit.MilliSecond) / 1000) * RenderControl.RenderContext.Timer.MotionFramePerSecond, ActionAfterMotion.Nothing);
            }
        }

        private void Np_PlayPaused()
        {
            if (motion != null && model != null)
            {
                motion.Stop();
            }
        }

        private void Np_PlayStarted()
        {
            if (model == null && motion == null)
            {
                Loader = new Thread(new ThreadStart(Load));
                Loader.Start();
            }
            else
            {
                throw new NullReferenceException("DanceLiteRenderer.Np_Playstared");
            }
        }

        private void Load()
        {
            if (!Util.TextTool.StringEmpty(pl.PMXPath) && !Util.TextTool.StringEmpty(pl.VMDPath))
            {
                Logger.Log("MMF Load Start");

                model = PMXModelWithPhysics.OpenLoad(System.IO.Path.Combine(pl.WorkingDirectory, pl.PMXPath), RenderControl.RenderContext);

                motion = model.MotionManager.AddMotionFromFile(System.IO.Path.Combine(pl.WorkingDirectory, pl.VMDPath), true);
                model.MotionManager.ApplyMotion(motion, (int)(np.GetPosition(TimeUnit.MilliSecond) / 1000) * RenderControl.RenderContext.Timer.MotionFramePerSecond, ActionAfterMotion.Nothing);
                //motion.Start((float)(np.GetPosition(TIMEUNIT.MSEC) / 1000) * RenderControl.RenderContext.Timer.MotionFramePerSecond, ActionAfterMotion.Nothing);

                RenderControl.WorldSpace.AddResource(model);

                RenderControl.TextureContext.MatrixManager.ViewMatrixManager.CameraPosition = new Vector3(0, 50, -25);
                RenderControl.TextureContext.MatrixManager.ViewMatrixManager.CameraLookAt = new Vector3(0, 10, 0);
                RenderControl.TextureContext.CameraMotionProvider = new CameraControl(RenderControl, 45);

                allowRender = true;

                Logger.Log("MMF Load Finish");
            }
        }

        private void Mw_MovingStateChanged(object sender, bool e)
        {
            if (RenderControl != null)
                RenderControl.AllowRendering = allowRender && !mw.isMoving && mw.isFrameUpadteAllowed;
        }

        private void Mw_UpdateAllowChanged(object sender, bool e)
        {
            if (RenderControl != null)
                RenderControl.AllowRendering = allowRender && !mw.isMoving && mw.isFrameUpadteAllowed;
        }

        public void Dispose()
        {
            mw.UpdateAllowChanged -= Mw_UpdateAllowChanged;
            mw.MovingStateChanged -= Mw_MovingStateChanged;

            mw = null;

            np.PlayPaused -= Np_PlayPaused;
            np.PlayResumed -= Np_PlayResumed;
            np.PlaySeeked -= Np_PlaySeeked;

            np = null;

            pl = null;

            RenderControl.AllowRendering = false;

            if (Loader != null)
            {
                Loader.Abort();
                Loader = null;
            }

            if (motion != null)
            {
                if (model != null)
                {
                    model.MotionManager.StopMotion(true);
                }
                motion.Stop();

                if (motion is MMDMotion)
                {
                    ((MMDMotion)motion).Dispose();
                }

                motion = null;
            }

            if (model != null)
            {
                RenderControl.WorldSpace.RemoveResource(model);

                model.BufferManager.Dispose();

                model.Dispose();

                model = null;
            }

            if (RenderControl != null)
            {
                RenderControl.WorldSpace.Dispose();

                RenderControl.Dispose();

                RenderControl = null;
            }

            Logger.Log("Dancer Lite Renderer is Closed. NEED FLUSHING");
        }
    }
}
