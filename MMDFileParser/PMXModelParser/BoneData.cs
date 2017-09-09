using SlimDX;
using System;
using System.Collections.Generic;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class BoneData
    {
        public string BoneName
        {
            get;
            private set;
        }

        public string BoneName_En
        {
            get;
            private set;
        }

        public Vector3 Position
        {
            get;
            private set;
        }

        public int ParentBoneIndex
        {
            get;
            private set;
        }

        public int TranslationLevel
        {
            get;
            private set;
        }

        public BoneConnectTo boneConnectTo
        {
            get;
            private set;
        }

        public bool canRotate
        {
            get;
            private set;
        }

        public bool canMove
        {
            get;
            private set;
        }

        public bool isVisible
        {
            get;
            private set;
        }

        public bool canOperate
        {
            get;
            private set;
        }

        public bool isIK
        {
            get;
            private set;
        }

        public LocalProvideTo localProvideTo
        {
            get;
            private set;
        }

        public bool isRotateProvided
        {
            get;
            private set;
        }

        public bool isMoveProvided
        {
            get;
            private set;
        }

        public bool isfixAxis
        {
            get;
            private set;
        }

        public bool isLocalAxis
        {
            get;
            private set;
        }

        public bool transformAfterPhysics
        {
            get;
            private set;
        }

        public bool ParentTransform
        {
            get;
            private set;
        }

        public Vector3 PositionOffset
        {
            get;
            private set;
        }

        public int ConnectedBoneIndex
        {
            get;
            private set;
        }

        public int ProvidedParentBoneIndex
        {
            get;
            private set;
        }

        public float ProvidedRatio
        {
            get;
            private set;
        }

        public Vector3 AxisDirectionVector
        {
            get;
            private set;
        }

        public Vector3 DimentionXDirectionVector
        {
            get;
            private set;
        }

        public Vector3 DimentionZDirectionVector
        {
            get;
            private set;
        }

        public int KeyValue
        {
            get;
            private set;
        }

        public int IKTargetBoneIndex
        {
            get;
            private set;
        }

        public int IKLoopNumber
        {
            get;
            private set;
        }

        public float IKLimitedRadian
        {
            get;
            private set;
        }

        public int IKLinkCount
        {
            get;
            private set;
        }

        public List<IkLinkData> ikLinks
        {
            get;
            private set;
        }

        internal static BoneData getBone(FileStream fs, Header header)
        {
            BoneData boneData = new BoneData();
            boneData.ikLinks = new List<IkLinkData>();
            boneData.BoneName = ParserHelper.getTextBuf(fs, header.Encode);
            boneData.BoneName_En = ParserHelper.getTextBuf(fs, header.Encode);
            boneData.Position = ParserHelper.getFloat3(fs);
            boneData.ParentBoneIndex = ParserHelper.getIndex(fs, header.BoneIndexSize);
            boneData.TranslationLevel = ParserHelper.getInt(fs);
            short chk = BitConverter.ToInt16(new byte[]
            {
                ParserHelper.getByte(fs),
                ParserHelper.getByte(fs)
            }, 0);
            boneData.boneConnectTo = (ParserHelper.isFlagEnabled(chk, 1) ? BoneConnectTo.Bone : BoneConnectTo.PositionOffset);
            boneData.canRotate = ParserHelper.isFlagEnabled(chk, 2);
            boneData.canMove = ParserHelper.isFlagEnabled(chk, 4);
            boneData.isVisible = ParserHelper.isFlagEnabled(chk, 8);
            boneData.canOperate = ParserHelper.isFlagEnabled(chk, 16);
            boneData.isIK = ParserHelper.isFlagEnabled(chk, 32);
            boneData.localProvideTo = (ParserHelper.isFlagEnabled(chk, 128) ? LocalProvideTo.ParentLocalTransformValue : LocalProvideTo.UserTransformValue);
            boneData.isRotateProvided = ParserHelper.isFlagEnabled(chk, 256);
            boneData.isMoveProvided = ParserHelper.isFlagEnabled(chk, 512);
            boneData.isfixAxis = ParserHelper.isFlagEnabled(chk, 1024);
            boneData.isLocalAxis = ParserHelper.isFlagEnabled(chk, 2048);
            boneData.transformAfterPhysics = ParserHelper.isFlagEnabled(chk, 4096);
            boneData.ParentTransform = ParserHelper.isFlagEnabled(chk, 8192);
            if (boneData.boneConnectTo == BoneConnectTo.PositionOffset)
            {
                boneData.PositionOffset = ParserHelper.getFloat3(fs);
            }
            else
            {
                boneData.ConnectedBoneIndex = ParserHelper.getIndex(fs, header.BoneIndexSize);
            }
            if (boneData.isRotateProvided || boneData.isMoveProvided)
            {
                boneData.ProvidedParentBoneIndex = ParserHelper.getIndex(fs, header.BoneIndexSize);
                boneData.ProvidedRatio = ParserHelper.getFloat(fs);
            }
            if (boneData.isfixAxis)
            {
                boneData.AxisDirectionVector = ParserHelper.getFloat3(fs);
            }
            if (boneData.isLocalAxis)
            {
                boneData.DimentionXDirectionVector = ParserHelper.getFloat3(fs);
                boneData.DimentionZDirectionVector = ParserHelper.getFloat3(fs);
            }
            if (boneData.ParentTransform)
            {
                boneData.KeyValue = ParserHelper.getInt(fs);
            }
            if (boneData.isIK)
            {
                boneData.IKTargetBoneIndex = ParserHelper.getIndex(fs, header.BoneIndexSize);
                boneData.IKLoopNumber = ParserHelper.getInt(fs);
                boneData.IKLimitedRadian = ParserHelper.getFloat(fs);
                boneData.IKLinkCount = ParserHelper.getInt(fs);
                for (int i = 0; i < boneData.IKLinkCount; i++)
                {
                    boneData.ikLinks.Add(IkLinkData.getIKLink(fs, header));
                }
            }
            return boneData;
        }
    }
}