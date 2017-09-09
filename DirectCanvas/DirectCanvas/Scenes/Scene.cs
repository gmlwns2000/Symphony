using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectCanvas.Misc;

namespace DirectCanvas.Scenes
{
    public abstract class Scene
    {
        public abstract void Render();
        
        public virtual void ActivateScene()
        { }
        public virtual void DeactivateScene()
        { }
        public virtual void SetInputState(int inputId, InputStatus status, PointF position)
        { }
    }
}
