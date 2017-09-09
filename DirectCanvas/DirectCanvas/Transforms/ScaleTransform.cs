using DirectCanvas.Misc;
using SlimDX;

namespace DirectCanvas.Transforms
{
    /// <summary>
    /// Scales an object in the 2-D coordinate system.
    /// </summary>
    public sealed class ScaleTransform : GeneralTransform
    {
        private float m_scaleX;
        private float m_scaleY;
        private float m_centerX;
        private float m_centerY;
        private bool m_isDirty = true;
        private Matrix3x2 m_cachedTransform;

        /// <summary>
        /// Gets or sets the x-axis scale factor.
        /// </summary>
        public float ScaleX
        {
            get { return m_scaleX; }
            set
            {
                m_scaleX = value;
                m_isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the y-axis scale factor.
        /// </summary>
        public float ScaleY
        {
            get { return m_scaleY; }
            set
            {
                m_scaleY = value;
                m_isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the center point of this ScaleTransform.
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
        /// Gets or sets the y-coordinate of the center point of this ScaleTransform.
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
        internal override Matrix3x2 GetTransformRelative(RectangleF bounds)
        {
            /* Get the relative scale on the X axis */
            float scaleX = bounds.Width*ScaleX;

            /* Get the relative scale on the Y axis */
            float scaleY = bounds.Height*ScaleY;

            /* Find the relative center on the X axis */
            float centerX = bounds.X + (CenterX * bounds.Width);
            /* Find the relative center on the Y axis */
            float centerY = bounds.Y + (CenterY * bounds.Height);

            /* Create the translation */
            var transform = Matrix3x2.Translation(centerX, centerY);

            /* Multiply our last transform by the scale */
            transform *= Matrix3x2.Scale(scaleX, scaleY);

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

            /* Create the translation */
            m_cachedTransform = Matrix3x2.Translation(CenterX, CenterY);

            /* Multiply our translation by the scale */
            m_cachedTransform *= Matrix3x2.Scale(ScaleX, ScaleY);

            /* Unset the dirty flag */
            m_isDirty = false;

            return m_cachedTransform;
        }
    }
}