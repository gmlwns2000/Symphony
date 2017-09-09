using ProtoBuf;
using System;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "SelfShadowFrame")]
    [Serializable]
    public class SelfShadowFrame : IExtensible
    {
        private ulong _frameNumber;

        private uint _type;

        private float _distance;

        private IExtension extensionObject;

        [ProtoMember(1, IsRequired = true, Name = "frameNumber", DataFormat = DataFormat.TwosComplement)]
        public ulong frameNumber
        {
            get
            {
                return _frameNumber;
            }
            set
            {
                _frameNumber = value;
            }
        }

        [ProtoMember(2, IsRequired = true, Name = "type", DataFormat = DataFormat.TwosComplement)]
        public uint type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        [ProtoMember(3, IsRequired = true, Name = "distance", DataFormat = DataFormat.FixedSize)]
        public float distance
        {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
