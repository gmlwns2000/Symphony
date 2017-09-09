using ProtoBuf;
using System;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "bvec2")]
    [Serializable]
    public class bvec2 : IExtensible
    {
        private uint _x;

        private uint _y;

        private IExtension extensionObject;

        [ProtoMember(1, IsRequired = true, Name = "x", DataFormat = DataFormat.TwosComplement)]
        public uint x
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        [ProtoMember(2, IsRequired = true, Name = "y", DataFormat = DataFormat.TwosComplement)]
        public uint y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
