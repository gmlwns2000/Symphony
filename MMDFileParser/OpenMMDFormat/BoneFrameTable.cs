using ProtoBuf;
using System;
using System.Collections.Generic;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "BoneFrameTable")]
    [Serializable]
    public class BoneFrameTable : IExtensible
    {
        private ulong _id;

        private readonly List<BoneFrame> _frames = new List<BoneFrame>();

        private IExtension extensionObject;

        [ProtoMember(1, IsRequired = true, Name = "id", DataFormat = DataFormat.TwosComplement)]
        public ulong id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        [ProtoMember(2, Name = "frames", DataFormat = DataFormat.Default)]
        public List<BoneFrame> frames
        {
            get
            {
                return _frames;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
