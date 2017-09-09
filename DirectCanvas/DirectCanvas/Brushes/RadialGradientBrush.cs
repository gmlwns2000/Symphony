using System.Collections.Generic;
using DirectCanvas.Misc;
using SlimDX.Direct2D;

namespace DirectCanvas.Brushes
{
    public class RadialGradientBrush : Brush
    {
        private readonly Direct2DRenderTarget m_renderTargetOwner;
        private readonly ExtendMode m_extendMode;
        private SlimDX.Direct2D.RadialGradientBrush m_internalRadialGradientBrush;
        private GradientStopCollection m_internalGradientStopCollection;
        private SizeF m_radius;
        private PointF m_centerPoint;
        private PointF m_gradientOriginOffset;

        internal RadialGradientBrush(Direct2DRenderTarget renderTargetOwner, 
                                     GradientStop[] gradientStops, 
                                     ExtendMode extendMode, 
                                     PointF centerPoint, 
                                     PointF gradientOriginOffset,
                                     SizeF radius)
        {
            m_renderTargetOwner = renderTargetOwner;
            m_extendMode = extendMode;
            m_radius = radius;
            m_gradientOriginOffset = gradientOriginOffset;
            m_centerPoint = centerPoint;

            var gradientStopList = new List<SlimDX.Direct2D.GradientStop>(gradientStops.Length);

            for (int i = 0; i < gradientStops.Length; i++)
            {
                gradientStopList.Add(gradientStops[i].InternalGradientStop);
            }

            var props = new RadialGradientBrushProperties();
            props.CenterPoint = centerPoint.InternalPointF;
            props.GradientOriginOffset = gradientOriginOffset.InternalPointF;
            props.HorizontalRadius = radius.Width;
            props.VerticalRadius = radius.Height;

            m_internalGradientStopCollection = new GradientStopCollection(m_renderTargetOwner.InternalRenderTarget,
                                                                          gradientStopList.ToArray(),
                                                                          Gamma.Linear,
                                                                          (SlimDX.Direct2D.ExtendMode)extendMode);

            m_internalRadialGradientBrush = new SlimDX.Direct2D.RadialGradientBrush(m_renderTargetOwner.InternalRenderTarget,
                                                                                    m_internalGradientStopCollection, props);
        }

        internal override SizeF BrushSize
        {
            get
            {
                return new SizeF(m_radius.Width * 2, m_radius.Height *2);
            }
        }

        public SizeF Radius
        {
            get { return m_radius; }
            set
            {
                m_radius = value;
                m_internalRadialGradientBrush.HorizontalRadius = value.Width;
                m_internalRadialGradientBrush.VerticalRadius = value.Height;
            }
        }

        public PointF GradientOriginOffset
        {
            get { return m_gradientOriginOffset; }
            set
            {
                m_gradientOriginOffset = value;
                m_internalRadialGradientBrush.GradientOriginOffset = m_gradientOriginOffset.InternalPointF;
            }
        }

        public PointF CenterPoint
        {
            get { return m_centerPoint; }
            set
            {
                m_centerPoint = value;
                m_internalRadialGradientBrush.CenterPoint = m_centerPoint.InternalPointF;
            }
        }

        public ExtendMode ExtendMode
        {
            get { return m_extendMode; }
        }

        protected override SlimDX.Direct2D.Brush GetInternalBrush()
        {
            return m_internalRadialGradientBrush;
        }

        public override void Dispose()
        {
            if (m_internalGradientStopCollection != null)
            {
                m_internalGradientStopCollection.Dispose();
                m_internalGradientStopCollection = null;
            }

            if(m_internalRadialGradientBrush != null)
            {
                m_internalRadialGradientBrush.Dispose();
                m_internalRadialGradientBrush = null;
            }
        }

        ~RadialGradientBrush()
        {
            Dispose();
        }
    }
}