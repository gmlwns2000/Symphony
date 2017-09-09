using MMDFileParser.PMXModelParser.JointParam;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class JointData
    {
        public string JointName
        {
            get;
            private set;
        }

        public string JointName_En
        {
            get;
            private set;
        }

        public JointType JointType
        {
            get;
            private set;
        }

        public JointParamBase JointParam
        {
            get;
            private set;
        }

        internal static JointData getJointData(Stream fs, Header header)
        {
            JointData jointData = new JointData();
            jointData.JointName = ParserHelper.getTextBuf(fs, header.Encode);
            jointData.JointName_En = ParserHelper.getTextBuf(fs, header.Encode);
            jointData.JointType = (JointType)ParserHelper.getByte(fs);
            jointData.JointParam = JointParamBase.GetJointParamBase(fs, header, jointData.JointType);
            return jointData;
        }
    }
}
