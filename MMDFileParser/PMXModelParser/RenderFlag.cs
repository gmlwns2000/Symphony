using System;

namespace MMDFileParser.PMXModelParser
{
    [Flags]
    public enum RenderFlag
    {
        CullNone = 1,
        GroundShadow = 2,
        RenderToZPlot = 4,
        RenderSelfShadow = 8,
        RenderEdge = 16
    }
}
