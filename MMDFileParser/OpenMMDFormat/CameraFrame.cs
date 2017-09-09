using ProtoBuf;
using System;
using System.ComponentModel;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "CameraFrame")]
    [Serializable]
    public class CameraFrame : IExtensible
    {
        private ulong _frameNumber;

        private vec3 _position;

        private vec3 _rotation;

        private float _distance;

        private BezInterpolParams _interpolParameters = null;

        private CameraExtraBezParams _cameraInterpolParams = null;

        private ulong _viewAngle;

        private uint _perspective;

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

        [ProtoMember(2, IsRequired = true, Name = "position", DataFormat = DataFormat.Default)]
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

        [ProtoMember(3, IsRequired = true, Name = "rotation", DataFormat = DataFormat.Default)]
        public vec3 rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        [ProtoMember(7, IsRequired = true, Name = "distance", DataFormat = DataFormat.FixedSize)]
        public float distance
        {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
            }
        }

        [ProtoMember(4, IsRequired = false, Name = "interpolParameters", DataFormat = DataFormat.Default), DefaultValue(null)]
        public BezInterpolParams interpolParameters
        {
            get
            {
                return _interpolParameters;
            }
            set
            {
                _interpolParameters = value;
            }
        }

        [ProtoMember(8, IsRequired = false, Name = "cameraInterpolParams", DataFormat = DataFormat.Default), DefaultValue(null)]
        public CameraExtraBezParams cameraInterpolParams
        {
            get
            {
                return _cameraInterpolParams;
            }
            set
            {
                _cameraInterpolParams = value;
            }
        }

        [ProtoMember(5, IsRequired = true, Name = "viewAngle", DataFormat = DataFormat.TwosComplement)]
        public ulong viewAngle
        {
            get
            {
                return _viewAngle;
            }
            set
            {
                _viewAngle = value;
            }
        }

        [ProtoMember(6, IsRequired = true, Name = "perspective", DataFormat = DataFormat.TwosComplement)]
        public uint perspective
        {
            get
            {
                return _perspective;
            }
            set
            {
                _perspective = value;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }
    }
}
