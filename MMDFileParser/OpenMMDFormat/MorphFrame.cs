using MMDFileParser;
using ProtoBuf;
using System;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "MorphFrame")]
    [Serializable]
    public class MorphFrame : IExtensible, IFrameData, IComparable
    {
        private ulong _frameNumber;

        private float _value;

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

        [ProtoMember(2, IsRequired = true, Name = "value", DataFormat = DataFormat.FixedSize)]
        public float value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public uint FrameNumber
        {
            get
            {
                return (uint)_frameNumber;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }

        public int CompareTo(object x)
        {
            return (int)(FrameNumber - ((IFrameData)x).FrameNumber);
        }
    }
}
