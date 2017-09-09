using DirectCanvas.Misc;
using SlimDX.Direct2D;

namespace DirectCanvas.Shapes
{
    public enum FillMode
    {
        Alternate = SlimDX.Direct2D.FillMode.Alternate,
        Winding = SlimDX.Direct2D.FillMode.Winding
    }

    public enum FigureBegin
    {
        Filled = SlimDX.Direct2D.FigureBegin.Filled,
        Hollow = SlimDX.Direct2D.FigureBegin.Hollow
    }

    public enum FigureEnd
    {
        Open = SlimDX.Direct2D.FigureEnd.Open,
        Closed = SlimDX.Direct2D.FigureEnd.Closed
    }

    public class PathGeometry : Geometry
    {
        private SlimDX.Direct2D.PathGeometry m_pathGeometry;
        private GeometrySink m_geometrySink;
        private bool m_geometryFilled = false;

        public bool Modifying { get; set; } = false;
        public bool Figuring { get; set; } = false;

        internal PathGeometry(Direct2DRenderTarget renderTargetResourceOwner) : base(renderTargetResourceOwner)
        {
            CreatePathGeometry();
        }

        private void CreatePathGeometry()
        {
            if(m_pathGeometry != null)
            {
                m_pathGeometry.Dispose();
                m_pathGeometry = null;
            }

            m_pathGeometry = new SlimDX.Direct2D.PathGeometry(InternalRenderTargetResourceOwner.InternalRenderTarget.Factory);
            m_geometryFilled = false;
        }

        protected override SlimDX.Direct2D.Geometry GetInternalGeometry()
        {
            return m_pathGeometry;
        }

        public void BeginModify(FillMode fillmode)
        {
            if(m_geometryFilled)
                CreatePathGeometry();

            m_geometrySink = m_pathGeometry.Open();
            m_geometrySink.SetFillMode((SlimDX.Direct2D.FillMode)fillmode);

            Modifying = true;
        }

        public void EndModify()
        {
            if (Modifying)
            {
                m_geometrySink.Close();
                m_geometrySink.Dispose();

                Modifying = false;
            }

            m_geometryFilled = true;
        }

        public void BeginFigure(FigureBegin figureBegin, PointF startPoint)
        {
            m_geometrySink.BeginFigure(startPoint.InternalPointF, (SlimDX.Direct2D.FigureBegin)figureBegin);

            Figuring = true;
        }

        public void EndFigure(FigureEnd figureEnd)
        {
            if (Figuring)
            {
                m_geometrySink.EndFigure((SlimDX.Direct2D.FigureEnd)figureEnd);
                Figuring = false;
            }
        }

        public void AddLine(PointF point)
        {
            m_geometrySink.AddLine(point.InternalPointF);
        }

        public void AddBezier(BezierSegment bezierSegment)
        {
            m_geometrySink.AddBezier(bezierSegment.InternalBezierSegment);
        }

        public void AddQuadraticBezier(PointF point1, PointF point2)
        {
            var segment = new QuadraticBezierSegment();
            segment.Point1 = point1;
            segment.Point2 = point2;
            m_geometrySink.AddQuadraticBezier(segment.InternalQuadraticBezierSegment);
        }

        public void AddArc(PointF endPoint, SizeF size, float rotationAngle)
        {
            var segment = new ArcSegment();
            segment.ArcSize = ArcSize.Small;
            segment.EndPoint = endPoint.InternalPointF;
            segment.RotationAngle = rotationAngle;
            segment.Size = size.InternalSize;
            segment.SweepDirection = SweepDirection.Clockwise;

            m_geometrySink.AddArc(segment);
        }

        public override void Dispose()
        {
            if (m_pathGeometry != null)
            {
                m_pathGeometry.Dispose();
                m_pathGeometry = null;
            }

            base.Dispose();
        }
    }
}