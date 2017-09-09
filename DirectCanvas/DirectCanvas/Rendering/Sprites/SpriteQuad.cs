using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DirectCanvas.Rendering.StreamBuffers;
using SlimDX;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D10.Device;

namespace DirectCanvas.Rendering.Sprites
{
    class SpriteQuad : GeometryMesh
    {
        private readonly StreamBuffer<VertexConstantData> m_vertexBatchConstantBuffer;
        private readonly VertexStreamBuffer<SpriteDrawData> m_instanceDataStream;
        private readonly List<IStreamBuffer> m_streamBuffers = new List<IStreamBuffer>();

        private int m_instanceCount = 0;

        public SpriteQuad(Device device, int maxInstances) : base(device)
        {
            int slot = 0;
            AddVetexStreamBuffer(new VertexStreamBuffer<SpriteVertex>(InternalDevice, 
                                                                      CreateQuadVertices(), 
                                                                      slot, 
                                                                      ResourceUsage.Default), 
                                                                      CreateInputElements());

         
            AddIndexStreamBuffer(CreateIndices());

            int totalBufferSize = 16;/* FIX ME: Size needs to be multiples of 16... or is it 8? */
            m_vertexBatchConstantBuffer = new ConstantStreamBuffer<VertexConstantData>(InternalDevice,
                                                                                       totalBufferSize,
                                                                                       0, 
                                                                                       ConstantStreamBufferType.VertexShader,
                                                                                       ResourceUsage.Dynamic,
                                                                                       CpuAccessFlags.Write);

            m_streamBuffers.Add(m_vertexBatchConstantBuffer);

            totalBufferSize = Marshal.SizeOf(typeof(SpriteDrawData)) * maxInstances;
            slot = 1;
            m_instanceDataStream = new VertexStreamBuffer<SpriteDrawData>(device,
                                                                          totalBufferSize,
                                                                          slot,
                                                                          ResourceUsage.Dynamic,
                                                                          CpuAccessFlags.Write);

            m_streamBuffers.Add(m_instanceDataStream);
        }

        public override void SetRenderState()
        {
            base.SetRenderState();

            foreach (var streamBuffer in m_streamBuffers)
            {
                streamBuffer.SetRenderState();
            }
        }

        public void SetConstants(VertexConstantData constants)
        {
            m_vertexBatchConstantBuffer.Write(constants);
        }

        public void SetInstanceData(SpriteDrawData[] drawData)
        {
            m_instanceDataStream.Write(drawData);
            m_instanceCount = drawData.Length;
        }

        public void BeginBatchInstanceData()
        {
            m_instanceCount = 0;
            m_instanceDataStream.BeginBatchWrite();
        }

        public void WriteBatchData(ref SpriteDrawData drawData)
        {
            m_instanceCount++;
            m_instanceDataStream.WriteBatch(ref drawData);
        }

        public void EndBatchInstanceData()
        {
            m_instanceDataStream.EndBatchWrite();
        }

        public void WriteInstanceData(IntPtr pData, int count)
        {
            m_instanceDataStream.Write(pData, count);
            m_instanceCount = count;
        }

        public void WriteInstanceData(ref SpriteDrawData[] data, int offset, int count)
        {
            m_instanceDataStream.WriteRange(ref data, offset, count);

            m_instanceCount = count;
        }

        /// <summary>
        /// Draws all the sprite instances
        /// </summary>
        public void Draw()
        {
            if (m_instanceCount == 0)
                return;

            InternalDevice.DrawIndexedInstanced(6, m_instanceCount, 0, 0, 0);
        }

        /// <summary>
        /// Creates the vertices for the quad
        /// </summary>
        /// <returns>Four vertices that make up a quad</returns>
        private static SpriteVertex[] CreateQuadVertices()
        {
            var verts = new SpriteVertex[4];
            verts[0] = new SpriteVertex { Position = new Vector2(0, 0), TexCoord = new Vector2(0, 0)};
            verts[1] = new SpriteVertex { Position = new Vector2(1, 0), TexCoord = new Vector2(1, 0)};
            verts[2] = new SpriteVertex { Position = new Vector2(1, 1), TexCoord = new Vector2(1, 1)};
            verts[3] = new SpriteVertex { Position = new Vector2(0, 1), TexCoord = new Vector2(0, 1)};

            return verts;
        }

        /// <summary>
        /// Creates the input elements that describe the vertices to the GPU
        /// </summary>
        private static ushort[] CreateIndices()
        {
            ushort[] indices = { 0, 1, 2, 3, 0, 2 };

            return indices;
        }

        /// <summary>
        /// Creates the input elements that describe the vertices to the GPU
        /// </summary>
        private InputElement[] CreateInputElements()
        {
            var layoutInstanced = new InputElement[10];

            layoutInstanced[0] = new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0, InputClassification.PerVertexData, 0);
            layoutInstanced[1] = new InputElement("TEXCOORD", 0, Format.R32G32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0);
            layoutInstanced[2] = new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1);
            layoutInstanced[3] = new InputElement("SOURCERECT", 0, Format.R32G32B32A32_Float, 16, 1, InputClassification.PerInstanceData, 1);
            layoutInstanced[4] = new InputElement("TEXTURESIZE", 0, Format.R32G32_Float, 32, 1, InputClassification.PerInstanceData, 1);
            layoutInstanced[5] = new InputElement("TEXTUREINDEX", 0, Format.R8_UInt, 40, 1, InputClassification.PerInstanceData, 1);
            layoutInstanced[6] = new InputElement("TRANSLATION", 0, Format.R32G32_Float, 44, 1, InputClassification.PerInstanceData, 1);
            layoutInstanced[7] = new InputElement("SCALEFACTOR", 0, Format.R32G32_Float, 52, 1, InputClassification.PerInstanceData, 1);
            layoutInstanced[8] = new InputElement("ROTATE", 0, Format.R32G32B32_Float, 60, 1, InputClassification.PerInstanceData, 1);
            layoutInstanced[9] = new InputElement("ROTATIONCENTER", 0, Format.R32G32_Float, 72, 1, InputClassification.PerInstanceData, 1);

            return layoutInstanced;
        }

        public override void Dispose()
        {
            foreach (var streamBuffer in m_streamBuffers)
            {
                streamBuffer.Dispose();
            }

            base.Dispose();
        }
    }
}
