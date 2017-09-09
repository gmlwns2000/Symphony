using System;
using System.Collections.Generic;
using DirectCanvas.Brushes;
using DirectCanvas.Helpers;
using DirectCanvas.Misc;
using DirectCanvas.Transforms;
using SlimDX;
using SlimDX.Direct2D;

namespace DirectCanvas.Shapes
{
    public abstract class Geometry : IDisposable
    {
        private readonly Direct2DRenderTarget m_renderTargetResourceOwner;
        private TransformedGeometry m_transformedGeometry;
        private Mesh m_mesh;

        internal Geometry(Direct2DRenderTarget renderTargetResourceOwner)
        {
            m_renderTargetResourceOwner = renderTargetResourceOwner;
        }

        public void Freeze()
        {
            if(m_mesh != null)
            {
                m_mesh.Dispose();
                m_mesh = null;
            }

            m_mesh = new Mesh(m_renderTargetResourceOwner.InternalRenderTarget);

            using(var tesselationSink = m_mesh.Open())
            {
                var geometry = GetInternalGeometry();
                SlimDX.Direct2D.Geometry.Tessellate(geometry, tesselationSink);
                tesselationSink.Close();
            }
        }

        public GeneralTransform Transform { get; set; }

        public RectangleF GetBounds()
        {
            var bounds = SlimDX.Direct2D.Geometry.GetBounds(GetCurrentGeometry());
            var ret = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            return ret;
        }

        public RectangleF GetBoundsInverted()
        {
            var bounds = SlimDX.Direct2D.Geometry.GetBounds(GetCurrentGeometry(), Matrix3x2.Invert(GetCurrentTransform()));
            var ret = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            return ret;
        }

        public RectangleF GetBoundsTransformed()
        {
            var bounds = SlimDX.Direct2D.Geometry.GetBounds(GetCurrentGeometry());
            var ret = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            return ret;
        }

        private Matrix3x2 GetCurrentTransform()
        {
            if (Transform == null)
                return Matrix3x2.Identity;

            return Transform.GetTransform();
        }

        private SlimDX.Direct2D.Geometry GetCurrentGeometry()
        {
            if (m_transformedGeometry != null)
                return m_transformedGeometry;

            return GetInternalGeometry();
        }

        internal void Fill(DrawingLayer drawingLayer, Brushes.Brush brush)
        {
            var bounds = GetBoundsInverted();
            
            BrushHelper.PrepareBrush(brush, drawingLayer, bounds, GetCurrentTransform(), Matrix3x2.Identity);

            if (m_transformedGeometry != null)
            {
                m_transformedGeometry.Dispose();
                m_transformedGeometry = null;
            }

            m_transformedGeometry = new TransformedGeometry(InternalRenderTargetResourceOwner.InternalRenderTarget.Factory, 
                                                            GetInternalGeometry(),
                                                            GetCurrentTransform());

            if (m_mesh == null)
                drawingLayer.D2DRenderTarget.InternalRenderTarget.FillGeometry(m_transformedGeometry, brush.InternalBrush);
            else
            {
                drawingLayer.D2DRenderTarget.InternalRenderTarget.AntialiasMode = AntialiasMode.Aliased;
                drawingLayer.D2DRenderTarget.InternalRenderTarget.FillMesh(m_mesh, brush.InternalBrush);
            }
        }

        internal void Draw(DrawingLayer drawingLayer, Brushes.Brush brush, float stroakWidth)
        {
            var bounds = GetBoundsInverted();

            BrushHelper.PrepareBrush(brush, drawingLayer, bounds, GetCurrentTransform(), Matrix3x2.Identity);

            if (m_transformedGeometry != null)
            {
                m_transformedGeometry.Dispose();
                m_transformedGeometry = null;
            }

            m_transformedGeometry = new TransformedGeometry(InternalRenderTargetResourceOwner.InternalRenderTarget.Factory,
                                                            GetInternalGeometry(),
                                                            GetCurrentTransform());

            if (m_mesh == null)
                drawingLayer.D2DRenderTarget.InternalRenderTarget.DrawGeometry(m_transformedGeometry, brush.InternalBrush, stroakWidth);
            else
            {
                drawingLayer.D2DRenderTarget.InternalRenderTarget.AntialiasMode = AntialiasMode.Aliased;
                drawingLayer.D2DRenderTarget.InternalRenderTarget.FillMesh(m_mesh, brush.InternalBrush);
            }
        }
        
        internal Direct2DRenderTarget InternalRenderTargetResourceOwner
        {
            get { return m_renderTargetResourceOwner; }
        }

        protected abstract SlimDX.Direct2D.Geometry GetInternalGeometry();

        public virtual void Dispose()
        {
            if(m_mesh != null)
            {
                m_mesh.Dispose();
                m_mesh = null;
            }

            if(m_transformedGeometry != null)
            {
                m_transformedGeometry.Dispose();
                m_transformedGeometry = null;
            }
        }
    }
}
