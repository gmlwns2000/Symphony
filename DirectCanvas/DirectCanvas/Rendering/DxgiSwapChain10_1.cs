using System;
using System.Collections.Generic;
using DirectCanvas.Rendering.Materials;
using SlimDX.Direct3D10;

namespace DirectCanvas.Rendering
{
    /// <summary>
    /// The DxgiSwapChain is an internal class used to handle DXGI swapchain
    /// specifics.  This specific subclass handles D3D v10.1
    /// </summary>
    class DxgiSwapChain10_1 : DxgiSwapChain
    {
        /// <summary>
        /// Internal list of DrawingLayers, backed by the SwapChains backbuffers
        /// </summary>
        private List<DrawingLayer> m_drawingLayers = new List<DrawingLayer>();

        /// <summary>
        /// Internal reference to the DirectCanvasFactory
        /// </summary>
        private DirectCanvasFactory m_directCanvasFactory;

        public DxgiSwapChain10_1(DirectCanvasFactory directCanvas, IntPtr hWnd, int width, int height) :
            base(directCanvas.DeviceContext.Device, directCanvas.DeviceContext.Direct3DFactory, hWnd, width, height)
        {
            m_directCanvasFactory = directCanvas;
        }

        /// <summary>
        /// Called when a resize of the swapchain backbuffers is called
        /// </summary>
        /// <param name="width">The pixel width</param>
        /// <param name="height">The pixel height</param>
        protected override void OnResizeBuffers(int width, int height)
        {
            /* Destory all of our old references! */
            foreach (var layer in m_drawingLayers)
            {
                layer.Dispose();
            }

            m_drawingLayers.Clear();
        }

        /// <summary>
        /// Returns the current backbuffer from the swapchain used for drawing
        /// </summary>
        public override DrawingLayer GetCurrentDrawingLayer()
        {
            /* Get the current back buffer */
            var backbuffer = Resource.FromSwapChain<Texture2D>(InternalSwapChain, 0);

            DrawingLayer ret;

            /* Search if we have delt with this backbuffer before because we would have already
             * wrapped it in our drawing layer and added it to our cache */
            foreach (var layer in m_drawingLayers)
            {
                ret = layer;
                /* I felt safe comparing pointers to make sure they are the same ref...*/
                if (layer.RenderTargetTexture.InternalTexture2D.ComPointer == backbuffer.ComPointer)
                    return ret;
            }

            /* Wrap our backbuffer in a DrawingLayer */
            ret = new DrawingLayer(m_directCanvasFactory, new RenderTargetTexture(backbuffer));

            /* Save it in our cache */
            m_drawingLayers.Add(ret);

            return ret;
        }

        public override void Dispose()
        {
            /* Destory all of our old references! */
            foreach (var layer in m_drawingLayers)
            {
                layer.Dispose();
            }

            m_drawingLayers.Clear();

            base.Dispose();
        }
    }
}
