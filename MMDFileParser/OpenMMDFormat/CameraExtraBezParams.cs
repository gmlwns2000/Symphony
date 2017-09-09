using ProtoBuf;
using System;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "CameraExtraBezParams")]
    [Serializable]
    public class CameraExtraBezParams : IExtensible
    {
        private bvec2 _L1;

        private bvec2 _L2;

        private bvec2 _V1;

        private bvec2 _V2;

        private IExtension extensionObject;

        [ProtoMember(1, IsRequired = true, Name = "L1", DataFormat = DataFormat.Default)]
        public bvec2 L1
        {
            get
            {
                return _L1;
            }
            set
            {
                _L1 = value;
            }
        }

        [ProtoMember(2, IsRequired = true, Name = "L2", DataFormat = DataFormat.Default)]
        public bvec2 L2
        {
            get
            {
                return _L2;
            }
            set
            {
                _L2 = value;
            }
        }

        [ProtoMember(3, IsRequired = true, Name = "V1", DataFormat = DataFormat.Default)]
        public bvec2 V1
        {
            get
            {
                return _V1;
            }
            set
            {
                _V1 = value;
            }
        }

        [ProtoMember(4, IsRequired = true, Name = "V2", DataFormat = DataFormat.Default)]
        public bvec2 V2
        {
            get
            {
                return _V2;
            }
            set
            {
                _V2 = value;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
