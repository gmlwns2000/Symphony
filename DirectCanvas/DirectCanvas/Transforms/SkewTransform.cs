using DirectCanvas.Misc;
using SlimDX;
using PointF = System.Drawing.PointF;

namespace DirectCanvas.Transforms
{
    /// <summary>
    /// A 2D skew transformation
    /// </summary>
    public sealed class SkewTransform : GeneralTransform
    {
        /// <summary>
        /// Gets or sets the x-coordinate of the transform center.
        /// </summary>
        public float CenterX { get; set; }

        /// <summary>
        /// Gets or sets the y-coordinate of the transform center.
        /// </summary>
        public float CenterY { get; set; }

        /// <summary>
        /// Gets or sets the y-axis skew angle, which is measured in degrees counterclockwise from the x-axis.
        /// </summary>
        public float AngleX { get; set; }

        /// <summary>
        /// Gets or sets the x-axis skew angle, which is measured in degrees counterclockwise from the y-axis.
        /// </summary>
        public float AngleY { get; set; }

        /// <summary>
        /// Internal method to get the transform relative to a bounding box
        /// </summary>
        /// <param name="bounds">The bounding box to transform relative to</param>
        /// <returns>A transform that is relative to a bounding box</returns>
        internal override Matrix3x2 GetTransformRelative(RectangleF bounds)
        {
            /* Find the relative center on the X axis */
            float centerX = bounds.X + (CenterX * bounds.Width);
            /* Find the relative center on the Y axis */
            float centerY = bounds.Y + (CenterY * bounds.Height);

            /* Create the skew transform */
            return Matrix3x2.Skew(AngleX, AngleY, new PointF(centerX, centerY));
        }

        /// <summary>
        /// Get the transform matrix for the given Transform
        /// </summary>
        internal override Matrix3x2 GetTransform()
        {
            return Matrix3x2.Skew(AngleX, AngleY, new PointF(CenterX, CenterY));
        }
    }
}