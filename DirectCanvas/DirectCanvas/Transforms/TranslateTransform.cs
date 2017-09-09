using DirectCanvas.Misc;
using SlimDX;

namespace DirectCanvas.Transforms
{
    /// <summary>
    /// Moves an object in the 2-D coordinate system.
    /// </summary>
    public sealed class TranslateTransform : GeneralTransform
    {
        private float m_x;
        private float m_y;
        private bool m_isDirty = true;
        private Matrix3x2 m_cachedTransform;

        /// <summary>
        /// Gets or sets the distance to move along the x-axis.
        /// </summary>
        public float X
        {
            get { return m_x; }
            set
            {
                m_x = value;
                m_isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the distance to move an object along the y-axis.
        /// </summary>
        public float Y
        {
            get { return m_y; }
            set
            {
                m_y = value;
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
            /* Find the relative center on the X axis */
            float centerX = bounds.X + (X * bounds.Width);

            /* Find the relative center on the Y axis */
            float centerY = bounds.Y + (Y * bounds.Height);

            /* Create the translation transform matrix */
            var transform = Matrix3x2.Translation(centerX, centerY);

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
            m_cachedTransform = Matrix3x2.Translation(X, Y);

            m_isDirty = false;

            return m_cachedTransform;
        }
    }
}