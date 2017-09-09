using DirectCanvas.Misc;
using SlimDX;

namespace DirectCanvas.Transforms
{
    /// <summary>
    /// A composite Transform composed of other GeneralTransform objects
    /// </summary>
    public sealed class TransformGroup : GeneralTransform
    {
        /// <summary>
        /// NEED TO IMPLEMENT CACHING ON THIS
        /// </summary>
        private Matrix3x2 m_cachedTransform;

        /// <summary>
        /// The child transformations
        /// </summary>
        private TransformCollection m_transformChildren = new TransformCollection();

        /// <summary>
        /// Gets  TransformCollection that defines this TransformGroup.
        /// </summary>
        public TransformCollection Children { get { return m_transformChildren; } }

        /// <summary>
        /// Internal method to get the transform relative to a bounding box
        /// </summary>
        /// <param name="bounds">The bounding box to transform relative to</param>
        /// <returns>A transform that is relative to a bounding box</returns>
        internal override Matrix3x2 GetTransformRelative(RectangleF bounds)
        {
            var xform = Matrix3x2.Identity;

            /* Loop over all of our child transforms */
            for (int i = 0; i < m_transformChildren.Count; i++)
            {
                var transform = m_transformChildren[i].GetTransformRelative(bounds);
                /* Multiple the transforms together */
                xform *= transform;
            }

            /* Return the composite result */
            return xform;
        }

        /// <summary>
        /// Get the transform matrix for the given Transform
        /// </summary>
        internal override Matrix3x2 GetTransform()
        {
            m_cachedTransform = Matrix3x2.Identity;

            /* Loop over all of our child transforms */
            for (int i = 0; i < m_transformChildren.Count; i++)
            {
                var transform = m_transformChildren[i].GetTransform();
                /* Multiple the transforms together */
                m_cachedTransform *= transform;
            }

            return m_cachedTransform;
        }
    }
}