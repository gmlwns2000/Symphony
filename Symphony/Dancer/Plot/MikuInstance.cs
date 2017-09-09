using MMF;
using MMF.Model.PMX;
using MMF.Motion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Dancer
{
    public class MikuInstance : Instance
    {
        string PMXPath;
        string VMDPath;
        PMXModel Model;
        IMotionProvider Motion;

        public MikuInstance(string Name, double startPosition, double duration, string PMXPath, string VMDPath) : base(Name, startPosition, duration)
        {
            this.Name = Name;
            StartPosition = startPosition;
            Duration = duration;
            this.PMXPath = PMXPath;
            this.VMDPath = VMDPath;
        }

        public void LoadMotion(string workingDirectory)
        {
            if (VMDPath != null&& Model!=null)
            {
                string file = Path.Combine(workingDirectory, VMDPath);
                if (File.Exists(file))
                {
                    Logger.Log("Start Motion Load: " + file);
                    Motion = Model.MotionManager.AddMotionFromFile(file, true);
                    Logger.Log("End Motion Load: " + file);
                }
            }
        }

        public void LoadModel(string workingDirectory, RenderContext context)
        {
            if(PMXPath!= null)
            {
                string file = Path.Combine(workingDirectory, PMXPath);
                Logger.Log( "Load Model" + file);
                if (File.Exists(file))
                {
                    Logger.Log("Start Model Load: " + file);
                    try
                    {
                        Model = PMXModelWithPhysics.OpenLoad(file, context);
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        Logger.Error(this, e);
                        Logger.Error("HALT Model Load");
                        Model = null;
                        return;
                    }
                    RenderControl.WorldSpace.AddResource(Model);
                    Logger.Log("End Motion Load: " + file);
                }
            }
        }

        MMF.Controls.WPF.WPFRenderControl RenderControl;
        public override void OnLoad(MMF.Controls.WPF.WPFRenderControl RenderControl, string workingDirectory)
        {
            this.RenderControl = RenderControl;

            LoadModel(workingDirectory, RenderControl.RenderContext);
            LoadMotion(workingDirectory);
        }

        public override void OnPlayStarted(double Position)
        {
            if(Motion!=null && Model != null)
            {
                Model.MotionManager.ApplyMotion(Motion, (int)Position / 24);
            }
        }

        public override void OnPauseChanged(bool IsPaused)
        {
            base.OnPauseChanged(IsPaused);
        }

        public override void OnSeeked(double NewPosition)
        {
            base.OnSeeked(NewPosition);
        }

        public override void Dispose()
        {
            if (Motion != null)
            {
                Motion.Stop();
            }

            if (Model != null)
            {
                RenderControl.WorldSpace.RemoveResource(Model);

                Model.Dispose();
            }
        }
    }
}
