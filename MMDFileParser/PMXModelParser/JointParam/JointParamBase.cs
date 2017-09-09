using System;
using System.IO;

namespace MMDFileParser.PMXModelParser.JointParam
{
    public abstract class JointParamBase
    {
        internal static JointParamBase GetJointParamBase(Stream fs, Header header, JointType type)
        {
            if (type != JointType.Spring6DOF)
            {
                throw new NotSupportedException("PMX2.0までしかサポートしてません。");
            }
            Spring6DofJointParam spring6DofJointParam = new Spring6DofJointParam();
            spring6DofJointParam.getJointParam(fs, header);
            return spring6DofJointParam;
        }

        internal abstract void getJointParam(Stream fs, Header header);
    }
}
