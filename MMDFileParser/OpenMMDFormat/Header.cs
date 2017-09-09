using ProtoBuf;
using System;
using System.Collections.Generic;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "Header")]
    [Serializable]
    public class Header : IExtensible
    {
        private string _versionInfo;

        private readonly List<string> _modelInfo = new List<string>();

        private IExtension extensionObject;

        [ProtoMember(1, IsRequired = true, Name = "versionInfo", DataFormat = DataFormat.Default)]
        public string versionInfo
        {
            get
            {
                return _versionInfo;
            }
            set
            {
                _versionInfo = value;
            }
        }

        [ProtoMember(2, Name = "modelInfo", DataFormat = DataFormat.Default)]
        public List<string> modelInfo
        {
            get
            {
                return _modelInfo;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
