using MMDFileParser;
using ProtoBuf;
using System;
using System.ComponentModel;

namespace OpenMMDFormat
{
    [ProtoContract(Name = "BoneFrame")]
    [Serializable]
    public class BoneFrame : IExtensible, IFrameData, IComparable
    {
        private ulong _frameNumber;

        private vec3 _position;

        private vec4 _rotation;

        private BezInterpolParams _interpolParameters = null;

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
        public vec4 rotation
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

        public uint FrameNumber
        {
            get
            {
                return (uint)_frameNumber;
            }
        }

        IExtension IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
        }

        public int CompareTo(object x)
        {
            return (int)(FrameNumber - ((IFrameData)x).FrameNumber);
        }
    }
}
