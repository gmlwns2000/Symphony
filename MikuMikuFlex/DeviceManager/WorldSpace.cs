using MMF.Model;
using MMF.Motion;
using MMF.Sprite;
using System;
using System.Collections.Generic;

namespace MMF.DeviceManager
{
    public class WorldSpace : IDisposable
    {
        private List<IDrawable> drawableResources = new List<IDrawable>();

        private List<IMovable> moveResources = new List<IMovable>();

        private List<IDynamicTexture> dynamicTextures = new List<IDynamicTexture>();

        private List<IGroundShadowDrawable> groundShadowDrawables = new List<IGroundShadowDrawable>();

        private List<IEdgeDrawable> edgeDrawables = new List<IEdgeDrawable>();

        public WorldSpace()
        {
        }

        private bool isDisposed;

        public List<IDrawable> DrawableResources
        {
            get
            {
                return drawableResources;
            }
            private set
            {
                drawableResources = value;
            }
        }

        public List<IMovable> MoveResources
        {
            get
            {
                return moveResources;
            }
            private set
            {
                moveResources = value;
            }
        }

        public List<IDynamicTexture> DynamicTextures
        {
            get
            {
                return dynamicTextures;
            }
            private set
            {
                dynamicTextures = value;
            }
        }

        public List<IGroundShadowDrawable> GroundShadowDrawables
        {
            get
            {
                return groundShadowDrawables;
            }
            private set
            {
                groundShadowDrawables = value;
            }
        }

        public List<IEdgeDrawable> EdgeDrawables
        {
            get
            {
                return edgeDrawables;
            }
            private set
            {
                edgeDrawables = value;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }

        public IEnumerable<object> DrawableGroups { get; internal set; }

        public void AddResource(IDrawable drawable)
        {
            drawableResources.Insert(0, drawable);
            if (drawable is IMovable)
            {
                moveResources.Add((IMovable)drawable);
            }
            if (drawable is IEdgeDrawable)
            {
                edgeDrawables.Add((IEdgeDrawable)drawable);
            }
            if (drawable is IGroundShadowDrawable)
            {
                groundShadowDrawables.Add((IGroundShadowDrawable)drawable);
            }
        }

        public void RemoveResource(IDrawable drawable)
        {
            if (drawableResources.Remove(drawable))
            {
                if (drawable is IMovable)
                {
                    moveResources.Remove((IMovable)drawable);
                }
            }
        }

        public void DrawAllResources()
        {
            foreach (IEdgeDrawable current in edgeDrawables)
            {
                if (current.Visibility)
                {
                    //current.DrawEdge();
                }
            }
            foreach (IDrawable current2 in DrawableResources)
            {
                if (current2.Visibility)
                {
                    current2.Draw();
                }
            }
            foreach (IGroundShadowDrawable current3 in groundShadowDrawables)
            {
                if (current3.Visibility)
                {
                    //TODO: DRAW GROUND !
                    //current3.DrawGroundShadow();
                }
            }
        }

        public void AddDynamicTexture(IDynamicTexture dtexture)
        {
            dynamicTextures.Add(dtexture);
        }

        public void RemoveDynamicTexture(IDynamicTexture dtexture)
        {
            if (dynamicTextures.Contains(dtexture))
            {
                dynamicTextures.Remove(dtexture);
            }
        }

        public void Dispose()
        {
            foreach (IDrawable current in DrawableResources)
            {
                if (current != null)
                {
                    current.Dispose();
                }
            }
            foreach (IDynamicTexture current2 in DynamicTextures)
            {
                if (dynamicTextures != null)
                {
                    current2.Dispose();
                }
            }
            isDisposed = true;
        }

        public void UpdateAllDynamicTexture()
        {
            foreach (IDynamicTexture current in DynamicTextures)
            {
                current.UpdateTexture();
            }
        }
    }
}
