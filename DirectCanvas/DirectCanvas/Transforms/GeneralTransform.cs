using DirectCanvas.Misc;
using SlimDX;

namespace DirectCanvas.Transforms
{
    /// <summary>
    /// This is an abstract class that provides generalized transformation support for geometry and brushes. 
    /// </summary>
    public abstract class GeneralTransform
    {
        /// <summary>
        /// Internal method to get the transform relative to a bounding box
        /// </summary>
        /// <param name="bounds">The bounding box to transform relative to</param>
        /// <returns>A transform that is relative to a bounding box</returns>
        internal abstract Matrix3x2 GetTransformRelative(RectangleF bounds);

        /// <summary>
        /// Get the transform matrix for the given Transform
        /// </summary>
        internal abstract Matrix3x2 GetTransform();
    }
}
