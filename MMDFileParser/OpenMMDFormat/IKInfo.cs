using ProtoBuf;
using System;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "IKInfo")]
    [Serializable]
    public class IKInfo : IExtensible
    {
        private ulong _id;

        private uint _isOn;

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

        [ProtoMember(2, IsRequired = true, Name = "isOn", DataFormat = DataFormat.TwosComplement)]
        public uint isOn
        {
            get
            {
                return _isOn;
            }
            set
            {
                _isOn = value;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
