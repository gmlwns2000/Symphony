using ProtoBuf;
using System;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "BezInterpolParams")]
    [Serializable]
    public class BezInterpolParams : IExtensible
    {
        private bvec2 _X1;

        private bvec2 _X2;

        private bvec2 _Y1;

        private bvec2 _Y2;

        private bvec2 _Z1;

        private bvec2 _Z2;

        private bvec2 _R1;

        private bvec2 _R2;

        private IExtension extensionObject;

        [ProtoMember(1, IsRequired = true, Name = "X1", DataFormat = DataFormat.Default)]
        public bvec2 X1
        {
            get
            {
                return _X1;
            }
            set
            {
                _X1 = value;
            }
        }

        [ProtoMember(2, IsRequired = true, Name = "X2", DataFormat = DataFormat.Default)]
        public bvec2 X2
        {
            get
            {
                return _X2;
            }
            set
            {
                _X2 = value;
            }
        }

        [ProtoMember(3, IsRequired = true, Name = "Y1", DataFormat = DataFormat.Default)]
        public bvec2 Y1
        {
            get
            {
                return _Y1;
            }
            set
            {
                _Y1 = value;
            }
        }

        [ProtoMember(4, IsRequired = true, Name = "Y2", DataFormat = DataFormat.Default)]
        public bvec2 Y2
        {
            get
            {
                return _Y2;
            }
            set
            {
                _Y2 = value;
            }
        }

        [ProtoMember(5, IsRequired = true, Name = "Z1", DataFormat = DataFormat.Default)]
        public bvec2 Z1
        {
            get
            {
                return _Z1;
            }
            set
            {
                _Z1 = value;
            }
        }

        [ProtoMember(6, IsRequired = true, Name = "Z2", DataFormat = DataFormat.Default)]
        public bvec2 Z2
        {
            get
            {
                return _Z2;
            }
            set
            {
                _Z2 = value;
            }
        }

        [ProtoMember(7, IsRequired = true, Name = "R1", DataFormat = DataFormat.Default)]
        public bvec2 R1
        {
            get
            {
                return _R1;
            }
            set
            {
                _R1 = value;
            }
        }

        [ProtoMember(8, IsRequired = true, Name = "R2", DataFormat = DataFormat.Default)]
        public bvec2 R2
        {
            get
            {
                return _R2;
            }
            set
            {
                _R2 = value;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
