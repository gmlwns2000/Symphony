using ProtoBuf;
using System;
using System.Collections.Generic;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "ShowIKFrame")]
    [Serializable]
    public class ShowIKFrame : IExtensible
    {
        private ulong _frame;

        private uint _show;

        private readonly List<IKInfo> _ikInfo = new List<IKInfo>();

        private IExtension extensionObject;

        [ProtoMember(1, IsRequired = true, Name = "frame", DataFormat = DataFormat.TwosComplement)]
        public ulong frame
        {
            get
            {
                return _frame;
            }
            set
            {
                _frame = value;
            }
        }

        [ProtoMember(2, IsRequired = true, Name = "show", DataFormat = DataFormat.TwosComplement)]
        public uint show
        {
            get
            {
                return _show;
            }
            set
            {
                _show = value;
            }
        }

        [ProtoMember(3, Name = "ikInfo", DataFormat = DataFormat.Default)]
        public List<IKInfo> ikInfo
        {
            get
            {
                return _ikInfo;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
