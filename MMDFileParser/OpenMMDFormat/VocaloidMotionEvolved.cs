using ProtoBuf;
using System;
using System.Collections.Generic;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "VocaloidMotionEvolved")]
    [Serializable]
    public class VocaloidMotionEvolved : IExtensible
    {
        private Header _header;

        private readonly List<IDTag> _boneIDTable = new List<IDTag>();

        private readonly List<IDTag> _morphIDTable = new List<IDTag>();

        private readonly List<IDTag> _cameraIDTable = new List<IDTag>();

        private readonly List<IDTag> _lightIDTable = new List<IDTag>();

        private readonly List<IDTag> _selfShadowIDTable = new List<IDTag>();

        private readonly List<BoneFrameTable> _boneFrameTables = new List<BoneFrameTable>();

        private readonly List<MorphFrameTable> _morphFrameTables = new List<MorphFrameTable>();

        private readonly List<CameraFrameTable> _cameraFrameTables = new List<CameraFrameTable>();

        private readonly List<LightFrameTable> _lightFrameTables = new List<LightFrameTable>();

        private readonly List<SelfShadowFrameTable> _selfShadowFrameTables = new List<SelfShadowFrameTable>();

        private readonly List<ShowIKFrame> _showIKFrames = new List<ShowIKFrame>();

        private IExtension extensionObject;

        [ProtoMember(1, IsRequired = true, Name = "header", DataFormat = DataFormat.Default)]
        public Header header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        [ProtoMember(2, Name = "boneIDTable", DataFormat = DataFormat.Default)]
        public List<IDTag> boneIDTable
        {
            get
            {
                return _boneIDTable;
            }
        }

        [ProtoMember(3, Name = "morphIDTable", DataFormat = DataFormat.Default)]
        public List<IDTag> morphIDTable
        {
            get
            {
                return _morphIDTable;
            }
        }

        [ProtoMember(4, Name = "cameraIDTable", DataFormat = DataFormat.Default)]
        public List<IDTag> cameraIDTable
        {
            get
            {
                return _cameraIDTable;
            }
        }

        [ProtoMember(5, Name = "lightIDTable", DataFormat = DataFormat.Default)]
        public List<IDTag> lightIDTable
        {
            get
            {
                return _lightIDTable;
            }
        }

        [ProtoMember(6, Name = "selfShadowIDTable", DataFormat = DataFormat.Default)]
        public List<IDTag> selfShadowIDTable
        {
            get
            {
                return _selfShadowIDTable;
            }
        }

        [ProtoMember(7, Name = "boneFrameTables", DataFormat = DataFormat.Default)]
        public List<BoneFrameTable> boneFrameTables
        {
            get
            {
                return _boneFrameTables;
            }
        }

        [ProtoMember(8, Name = "morphFrameTables", DataFormat = DataFormat.Default)]
        public List<MorphFrameTable> morphFrameTables
        {
            get
            {
                return _morphFrameTables;
            }
        }

        [ProtoMember(9, Name = "cameraFrameTables", DataFormat = DataFormat.Default)]
        public List<CameraFrameTable> cameraFrameTables
        {
            get
            {
                return _cameraFrameTables;
            }
        }

        [ProtoMember(10, Name = "lightFrameTables", DataFormat = DataFormat.Default)]
        public List<LightFrameTable> lightFrameTables
        {
            get
            {
                return _lightFrameTables;
            }
        }

        [ProtoMember(11, Name = "selfShadowFrameTables", DataFormat = DataFormat.Default)]
        public List<SelfShadowFrameTable> selfShadowFrameTables
        {
            get
            {
                return _selfShadowFrameTables;
            }
        }

        [ProtoMember(12, Name = "showIKFrames", DataFormat = DataFormat.Default)]
        public List<ShowIKFrame> showIKFrames
        {
            get
            {
                return _showIKFrames;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
