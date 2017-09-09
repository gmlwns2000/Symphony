using ProtoBuf;
using System;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "LightFrame")]
    [Serializable]
    public class LightFrame : IExtensible
    {
        private ulong _frameNumber;

        private vec3 _color;

        private vec3 _position;

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

        [ProtoMember(2, IsRequired = true, Name = "color", DataFormat = DataFormat.Default)]
        public vec3 color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        [ProtoMember(3, IsRequired = true, Name = "position", DataFormat = DataFormat.Default)]
        public vec3 position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
