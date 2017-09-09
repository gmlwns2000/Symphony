using System;
using DirectCanvas.Misc;
using DirectCanvas.Rendering;
using DirectCanvas.Rendering.Materials;
using DirectCanvas.Rendering.Sprites;

namespace DirectCanvas
{
    /// <summary>
    /// The Composer is an internal class that wraps the SpriteRenderer class
    /// Should consider merging with the SpriteRenderer class itself.
    /// </summary>
    class Composer : IDisposable
    {
        private const int MAX_SPRITE_INSTANCES = 20000;

        /// <summary>
        /// The real engine that renders and composes
        /// </summary>
        private SpriteRenderer m_spriteRenderer;
        private DrawStateManagement m_drawStateManagement;

        public Composer(DeviceContext10_1 deviceContext)
        {
            m_drawStateManagement = new DrawStateManagement();
            m_spriteRenderer = new SpriteRenderer(deviceContext.Device, MAX_SPRITE_INSTANCES);
        }

        /// <summary>
        /// Begin our composition process
        /// </summary>
        /// <param name="renderTargetTexture">The render target to write to</param>
        /// <param name="blendStateMode">The blend state to use</param>
        public void BeginDraw(RenderTargetTexture renderTargetTexture, BlendStateMode blendStateMode)
        {
            m_drawStateManagement.BeginDrawState();
            m_spriteRenderer.BeginDraw(FilterMode.Linear, renderTargetTexture, blendStateMode);
        }

        /// <summary>
        /// Ends the composition process and flushes any remaining composition commands
        /// </summary>
        public void EndDraw()
        {
            m_drawStateManagement.EndDrawState();
            m_spriteRenderer.EndDraw();
        }

        /// <summary>
        /// Composes a DrawingLayer with another DrawingLayer.  
        /// This allows for things like scaling, blending, 2D and 3D transformations
        /// </summary>
        /// <param name="layer">The DrawingLayer that is used as the input</param>
        /// <param name="sourceArea">The area over the input DrawingLayer</param>
        /// <param name="destinationArea">The output area to draw the source area</param>
        /// <param name="rotationTransform">The rotation parameters</param>
        /// <param name="tint">The color to tint the source layer on to the output</param>
        public void ComposeDrawingLayer(DrawingLayer layer, ref RectangleF sourceArea, ref RectangleF destinationArea, ref RotationParameters rotationTransform, ref Color4 tint)
        {
            m_drawStateManagement.DrawPreamble();

            /* Get the current draw data to fill.  We avoid creating new object instances
             * for the GC to have to delete */
            var spriteRenderData = m_spriteRenderer.GetCurrentRenderData();

            /* Copy all the primitive transform data to the spriteRenderData */

            float xDestination = destinationArea.X;
            float yDestination = destinationArea.Y;

            spriteRenderData.drawData.Translation.X = xDestination;
            spriteRenderData.drawData.Translation.Y = yDestination;

            spriteRenderData.drawData.Rotate.X = rotationTransform.RotateX;
            spriteRenderData.drawData.Rotate.Y = rotationTransform.RotateY;
            spriteRenderData.drawData.Rotate.Z = rotationTransform.RotateZ;

            spriteRenderData.drawData.Scale.X = destinationArea.Width / sourceArea.Width;
            spriteRenderData.drawData.Scale.Y = destinationArea.Height / sourceArea.Height;

            spriteRenderData.drawData.DrawRect.X = sourceArea.X;
            spriteRenderData.drawData.DrawRect.Y = sourceArea.Y;
            spriteRenderData.drawData.DrawRect.Z = sourceArea.Width;
            spriteRenderData.drawData.DrawRect.W = sourceArea.Height;

            spriteRenderData.drawData.RotationCenter = rotationTransform.RotationCenter.InternalVector2;

            spriteRenderData.drawData.Color = tint.InternalColor4;
            spriteRenderData.texture = layer.RenderTargetTexture;

            /* Add (queue) the drawing data to the sprite renderer */
            m_spriteRenderer.AddRenderData(spriteRenderData);
        }

        /// <summary>
        /// Composes a DrawingLayer with another DrawingLayer.  
        /// This allows for things like scaling, blending, 2D and 3D transformations
        /// </summary>
        /// <param name="layer">The DrawingLayer that is used as the input</param>
        /// <param name="sourceArea">The area over the input DrawingLayer</param>
        /// <param name="destinationArea">The output area to draw the source area</param>
        /// <param name="rotationTransform">The rotation parameters</param>
        /// <param name="tint">The color to tint the source layer on to the output</param>
        public void ComposeDrawingLayer(DrawingLayer layer, RectangleF sourceArea, RectangleF destinationArea, RotationParameters rotationTransform, Color4 tint)
        {
            ComposeDrawingLayer(layer, ref sourceArea, ref destinationArea, ref rotationTransform, ref tint);
        }

        public void Dispose()
        {
            if(m_spriteRenderer != null)
            {
                m_spriteRenderer.Dispose();
                m_spriteRenderer = null;
            }
        }
    }
}
