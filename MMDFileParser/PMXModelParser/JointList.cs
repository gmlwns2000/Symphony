using System.Collections.Generic;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class JointList
    {
        public int JointCount
        {
            get;
            private set;
        }

        public List<JointData> Joints
        {
            get;
            private set;
        }

        internal static JointList getJointList(Stream fs, Header header)
        {
            JointList jointList = new JointList();
            jointList.JointCount = ParserHelper.getInt(fs);
            jointList.Joints = new List<JointData>();
            for (int i = 0; i < jointList.JointCount; i++)
            {
                jointList.Joints.Add(JointData.getJointData(fs, header));
            }
            return jointList;
        }
    }
}
