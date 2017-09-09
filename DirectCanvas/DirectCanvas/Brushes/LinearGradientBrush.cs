using System;
using System.Collections.Generic;
using DirectCanvas.Misc;
using SlimDX.Direct2D;

namespace DirectCanvas.Brushes
{
    public class LinearGradientBrush : Brush
    {
        private readonly Direct2DRenderTarget m_renderTargetOwner;
        private SlimDX.Direct2D.LinearGradientBrush m_internalLinearGradientBrush;
        private GradientStopCollection m_internalGradientStopCollection;

        private PointF m_startPoint;
        private PointF m_endPoint;

        internal LinearGradientBrush(Direct2DRenderTarget renderTargetOwner, 
                                     GradientStop[] gradientStops, 
                                     ExtendMode extendMode, 
                                     PointF startPoint, 
                                     PointF endPoint)
        {
            m_renderTargetOwner = renderTargetOwner;

            var gradientStopList = new List<SlimDX.Direct2D.GradientStop>(gradientStops.Length);

            for (int i = 0; i < gradientStops.Length; i++)
            {
                gradientStopList.Add(gradientStops[i].InternalGradientStop);
            }

            var props = new LinearGradientBrushProperties();
            props.StartPoint = startPoint.InternalPointF;
            props.EndPoint = endPoint.InternalPointF;

            m_startPoint = startPoint;
            m_endPoint = endPoint;

            var internalRt = m_renderTargetOwner.InternalRenderTarget;

            m_internalGradientStopCollection = new GradientStopCollection(internalRt, 
                                                                          gradientStopList.ToArray(), 
                                                                          Gamma.Linear, 
                                                                          (SlimDX.Direct2D.ExtendMode)extendMode);

            m_internalLinearGradientBrush = new SlimDX.Direct2D.LinearGradientBrush(internalRt,
                                                                                    m_internalGradientStopCollection, 
                                                                                    props);
        }

        private static float GetDistance(PointF point1, PointF point2)
        {
            float a =(point2.X - point1.X);
            float b = (point2.Y - point1.Y);

            return (float)Math.Sqrt(a * a + b * b);
        }

        internal override SizeF BrushSize
        {
            get
            {
                float distance = GetDistance(m_endPoint, m_startPoint);

                /* Because this is a linear brush, it really doesn't have width/height, but
                 * a non-zero value seems required to render correctly */
                return new SizeF(distance, distance);
            }
        }

        public PointF StartPoint
        {
            get { return m_startPoint; }
            set
            {
                m_startPoint = value;
                m_internalLinearGradientBrush.StartPoint = value.InternalPointF;
            }
        }

        public PointF EndPoint
        {
            get { return m_endPoint; }
            set
            {
                m_endPoint = value;
                m_internalLinearGradientBrush.EndPoint = value.InternalPointF;
            }
        }

        protected override SlimDX.Direct2D.Brush GetInternalBrush()
        {
            return m_internalLinearGradientBrush;
        }

        public override void Dispose()
        {
            if(m_internalLinearGradientBrush != null)
            {
                m_internalLinearGradientBrush.Dispose();
                m_internalLinearGradientBrush = null;
            }

            if (m_internalGradientStopCollection != null)
            {
                m_internalGradientStopCollection.Dispose();
                m_internalGradientStopCollection = null;
            }
        }

        ~LinearGradientBrush()
        {
            Dispose();
        }
    }
}