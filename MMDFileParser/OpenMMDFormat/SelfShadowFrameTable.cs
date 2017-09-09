using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "SelfShadowFrameTable")]
    [Serializable]
    public class SelfShadowFrameTable : IExtensible
    {
        private ulong _id = 0uL;

        private readonly List<SelfShadowFrame> _frames = new List<SelfShadowFrame>();

        private IExtension extensionObject;

        [ProtoMember(1, IsRequired = false, Name = "id", DataFormat = DataFormat.TwosComplement), DefaultValue(0f)]
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
        public List<SelfShadowFrame> frames
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
