using SlimDX;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class IkLinkData
    {
        public int LinkBoneIndex
        {
            get;
            private set;
        }

        public bool isRotateLimited
        {
            get;
            private set;
        }

        public Vector3 MinimumRadian
        {
            get;
            private set;
        }

        public Vector3 MaximumRadian
        {
            get;
            private set;
        }

        internal static IkLinkData getIKLink(FileStream fs, Header header)
        {
            IkLinkData ikLinkData = new IkLinkData();
            ikLinkData.LinkBoneIndex = ParserHelper.getIndex(fs, header.BoneIndexSize);
            ikLinkData.isRotateLimited = (ParserHelper.getByte(fs) == 1);
            if (ikLinkData.isRotateLimited)
            {
                ikLinkData.MinimumRadian = ParserHelper.getFloat3(fs);
                ikLinkData.MaximumRadian = ParserHelper.getFloat3(fs);
            }
            return ikLinkData;
        }
    }
}
