namespace DirectCanvas
{
    /// <summary>
    /// BlendState modes used for composition
    /// </summary>
    public enum BlendStateMode
    {
        /// <summary>
        /// Vanilla alpha blend
        /// </summary>
        AlphaBlend,
        /// <summary>
        /// The colors are additive
        /// </summary>
        Additive,
        /// <summary>
        /// The colors are subtractive
        /// </summary>
        Subtractive,
        Copy,
        SourceOver,
        SourceATop,
        SourceIn,
        SourceOut,
        DestinationOver,
        DestinationATop,
        DestinationIn,
        DestinationOut
    }
}