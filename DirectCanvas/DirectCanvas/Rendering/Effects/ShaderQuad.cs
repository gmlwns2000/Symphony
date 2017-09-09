using DirectCanvas.Rendering.StreamBuffers;
using SlimDX;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D10.Device;

namespace DirectCanvas.Rendering.Effects
{
    /// <summary>
    /// A single instance quad used by the ShaderRenderer
    /// to draw on a render target using a pixel shader effect
    /// </summary>
    class ShaderQuad : GeometryMesh 
    {
        public ShaderQuad(Device device) : base(device)
        {
            int slot = 0;

            /* Run our helper method to create a vertex buffer
             * using our QuadVertex vertex structure */
            AddVetexStreamBuffer(new VertexStreamBuffer<QuadVertex>(InternalDevice, 
                                 CreateQuadVertices(), 
                                 slot, 
                                 ResourceUsage.Default), 
                                 CreateInputElements());

            AddIndexStreamBuffer(CreateIndices());
        }

        /// <summary>
        /// Creates the vertices for the quad
        /// </summary>
        /// <returns>Four vertices that make up a quad</returns>
        private static QuadVertex[] CreateQuadVertices()
        {
            var verts = new QuadVertex[4];

            verts[0] = new QuadVertex{Position = new Vector4(1, 1, 1, 1), TexCoord = new Vector2(1,0)};
            verts[1] = new QuadVertex { Position = new Vector4(1, -1, 1, 1), TexCoord = new Vector2(1, 1) };
            verts[2] = new QuadVertex { Position = new Vector4(-1, -1, 1, 1), TexCoord = new Vector2(0, 1) };
            verts[3] = new QuadVertex { Position = new Vector4(-1, 1, 1, 1), TexCoord = new Vector2(0, 0) };

            return verts;
        }

        /// <summary>
        /// Creates the input elements that describe the vertices to the GPU
        /// </summary>
        private static InputElement[] CreateInputElements()
        {
            var layoutInstanced = new InputElement[2];

            layoutInstanced[0] = new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0);
            layoutInstanced[1] = new InputElement("TEXCOORD", 0, Format.R32G32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0);

            return layoutInstanced;
        }

        /// <summary>
        /// Create the indices for the quad vertices
        /// </summary>
        /// <returns></returns>
        private static ushort[] CreateIndices()
        {
            ushort[] indices = { 0, 1, 2, 2, 3, 0 };

            return indices;
        }

        /// <summary>
        /// Draws the quad on the set render target
        /// </summary>
        public void Draw()
        {
            InternalDevice.DrawIndexed(6, 0, 0);
        }

    }
}
