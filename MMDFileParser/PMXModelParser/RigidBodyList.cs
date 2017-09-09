using System.Collections.Generic;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class RigidBodyList
    {
        public int RigidBodyCount
        {
            get;
            private set;
        }

        public List<RigidBodyData> RigidBodies
        {
            get;
            private set;
        }

        internal static RigidBodyList GetRigidBodyList(Stream fs, Header header)
        {
            RigidBodyList rigidBodyList = new RigidBodyList();
            rigidBodyList.RigidBodies = new List<RigidBodyData>();
            rigidBodyList.RigidBodyCount = ParserHelper.getInt(fs);
            for (int i = 0; i < rigidBodyList.RigidBodyCount; i++)
            {
                rigidBodyList.RigidBodies.Add(RigidBodyData.GetRigidBodyData(fs, header));
            }
            return rigidBodyList;
        }
    }
}
