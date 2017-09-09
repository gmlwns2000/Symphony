using ProtoBuf;
using System;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "IDTag")]
    [Serializable]
    public class IDTag : IExtensible
    {
        private ulong _id;

        private string _name;

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

        [ProtoMember(2, IsRequired = true, Name = "name", DataFormat = DataFormat.Default)]
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
