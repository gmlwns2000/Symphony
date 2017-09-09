using ProtoBuf;
using System;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "vec2")]
    [Serializable]
    public class vec2 : IExtensible
    {
        private float _x;

        private float _y;

        private IExtension extensionObject;

        [ProtoMember(1, IsRequired = true, Name = "x", DataFormat = DataFormat.FixedSize)]
        public float x
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

        [ProtoMember(2, IsRequired = true, Name = "y", DataFormat = DataFormat.FixedSize)]
        public float y
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
