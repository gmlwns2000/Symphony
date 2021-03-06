using ProtoBuf;
using System;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "vec4")]
    [Serializable]
    public class vec4 : IExtensible
    {
        private float _x;

        private float _y;

        private float _z;

        private float _w;

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

        [ProtoMember(3, IsRequired = true, Name = "z", DataFormat = DataFormat.FixedSize)]
        public float z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }

        [ProtoMember(4, IsRequired = true, Name = "w", DataFormat = DataFormat.FixedSize)]
        public float w
        {
            get
            {
                return _w;
            }
            set
            {
                _w = value;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
