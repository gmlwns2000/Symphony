using System.Drawing;
using SlimDX;

namespace DirectCanvas.Transforms
{
    /// <summary>
    /// Rotates an object clockwise about a specified point in a 2-D coordinate system.
    /// </summary>
    public sealed class RotateTransform : GeneralTransform
    {
        private float m_centerY;
        private float m_centerX;
        private float m_angle;
        private bool m_isDirty = true;
        private Matrix3x2 m_cachedTransform;

        /// <summary>
        /// Gets or sets the angle, in degrees, of clockwise rotation.
        /// </summary>
        public float Angle
        {
            get { return m_angle; }
            set
            {
                m_angle = value;
                m_isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the rotation center point.
        /// </summary>
        public float CenterX
        {
            get { return m_centerX; }
            set
            {
                m_centerX = value;
                m_isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the y-coordinate of the rotation center point.
        /// </summary>
        public float CenterY
        {
            get { return m_centerY; }
            set
            {
                m_centerY = value;
                m_isDirty = true;
            }
        }

        /// <summary>
        /// Internal method to get the transform relative to a bounding box
        /// </summary>
        /// <param name="bounds">The bounding box to transform relative to</param>
        /// <returns>A transform that is relative to a bounding box</returns>
        internal override Matrix3x2 GetTransformRelative(Misc.RectangleF bounds)
        {
            /* Find the relative center on the X axis */
            float centerX = bounds.X + (CenterX * bounds.Width);

            /* Find the relative center on the Y axis */
            float centerY = bounds.Y + (CenterY * bounds.Height);

            /* Create the rotation transform matrix */
            var transform = Matrix3x2.Rotation(Angle, new PointF(centerX, centerY));
          
            return transform;
        }

        /// <summary>
        /// Get the transform matrix for the given Transform
        /// </summary>
        internal override Matrix3x2 GetTransform()
        {
            /* Avoid re-creating the transform if nothing has change
             * to help optimize for high perf scenarios */
            if (!m_isDirty)
                return m_cachedTransform;

            /* Create the rotation */
            m_cachedTransform = Matrix3x2.Rotation(Angle, new PointF(CenterX, CenterY));
            
            /* Flag that we are not dirty */
            m_isDirty = false;

            return m_cachedTransform;
        }
    }
}