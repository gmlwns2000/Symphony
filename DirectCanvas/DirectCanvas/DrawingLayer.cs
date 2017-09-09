using System;
using System.Collections.Generic;
using DirectCanvas.Effects;
using DirectCanvas.Helpers;
using DirectCanvas.Imaging;
using DirectCanvas.Misc;
using DirectCanvas.Multimedia;
using DirectCanvas.Rendering.Materials;
using DirectCanvas.Shapes;
using DirectCanvas.Transforms;
using SlimDX;
using SlimDX.Direct2D;
using SlimDX.Direct3D10;
using SlimDX.DirectWrite;
using SlimDX.DXGI;
using Brush = DirectCanvas.Brushes.Brush;
using Pen = DirectCanvas.Brushes.Pen;
using Factory = SlimDX.DirectWrite.Factory;
using FactoryType = SlimDX.DirectWrite.FactoryType;
using FontWeight = SlimDX.DirectWrite.FontWeight;

namespace DirectCanvas
{
    /// <summary>
    /// The DrawingLayer is a surface in which drawing and rendering can take place.
    /// </summary>
    public class DrawingLayer : IDisposable
    {
        /// <summary>
        /// Reference to the DirectCanvasFactory
        /// </summary>
        private readonly DirectCanvasFactory m_directCanvasFactory;

        /// <summary>
        /// The color format of the surface
        /// </summary>
        private readonly Format m_format;

        /// <summary>
        /// The Direct3D backed render target
        /// </summary>
        private RenderTargetTexture m_renderTargetTexture;
        
        /// <summary>
        /// The Direct2D wrapped render target for the Direct3D surface (m_renderTargetTexture)
        /// </summary>
        private Direct2DRenderTarget m_d2DRenderTarget;

        private DrawStateManagement m_drawStateManagement;

        private bool m_enableImageCopy = false;

        /// <summary>
        /// The Event that called each Draw* functions in this Layer
        /// </summary>
        public event EventHandler<DrawCalledArgs> DrawCalled;

        public static event EventHandler<DrawCalledArgs> WorldDrawCalled;

        /// <summary>
        /// Pointer to the Direct3D Texture2D. Use this to render to the DrawingLayer
        /// using other Direct3D libraries.
        /// </summary>
        public IntPtr Texture2DComPointer
        {
            get
            {
                if (m_renderTargetTexture == null ||
                    m_renderTargetTexture.InternalTexture2D == null)
                    return IntPtr.Zero;
                return m_renderTargetTexture.InternalTexture2D.ComPointer;
            }
        }

        /// <summary>
        /// Pointer to the Direct3D10 RenderTargetView. Use this to render to the DrawingLayer
        /// using other Direct3D libraries.
        /// </summary>
        public IntPtr RenderTargetViewComPointer
        {
            get
            {
                if (m_renderTargetTexture == null ||
                    m_renderTargetTexture.InternalRenderTargetView == null)
                    return IntPtr.Zero;
                return m_renderTargetTexture.InternalRenderTargetView.ComPointer;
            }
        }

        public bool EnableImageCopy
        {
            get { return m_enableImageCopy; }
            set
            {
                if(m_enableImageCopy != value)
                {
                    m_enableImageCopy = value;
                    OnEnableImageCopyChanged();
                }
            }
        }

        public bool IsBeginDraw
        {
            get
            {
                return m_drawStateManagement.BeganDraw;
            }
        }

        /// <summary>
        /// The width, in pixels, of the DrawingLayer
        /// </summary>
        public int Width
        {
            get { return RenderTargetTexture.Description.Width; }
        }

        /// <summary>
        /// The height, in pixels, of the DrawingLayer
        /// </summary>
        public int Height
        {
            get { return RenderTargetTexture.Description.Height; }
        }

        public DirectCanvasFactory Factory
        {
            get { return m_directCanvasFactory; }
        }

        /// <summary>
        /// The Direct2D render target wrapper
        /// </summary>
        virtual internal Direct2DRenderTarget D2DRenderTarget
        {
            get { return m_d2DRenderTarget; }
            private set { m_d2DRenderTarget = value; }
        }

        /// <summary>
        /// The Direct3D texture wrapper
        /// </summary>
        virtual internal RenderTargetTexture RenderTargetTexture
        {
            get { return m_renderTargetTexture; }
            private set { m_renderTargetTexture = value; }
        }

        internal StagingTexture SystemMemoryTexture { get; private set; }

        #region Initializer

        /// <summary>
        /// Creates a new instance of the DrawingLayer
        /// </summary>
        /// <param name="directCanvasFactory">The factory that the DrawingLayer belongs to</param>
        /// <param name="width">The pixel size in width to make the D3D texture</param>
        /// <param name="height">The pixel size in height to make the D3D texture</param>
        /// <param name="format">The color format to make the D3D texture</param>
        /// <param name="optionFlags">Option flags to use on the creation of the D3D texture</param>
        internal DrawingLayer(DirectCanvasFactory directCanvasFactory, int width,  int height, 
                              Format format = Format.B8G8R8A8_UNorm, ResourceOptionFlags optionFlags = ResourceOptionFlags.None)
        {
            m_directCanvasFactory = directCanvasFactory;
            m_format = format;

            m_drawStateManagement = new DrawStateManagement();

            var device = m_directCanvasFactory.DeviceContext.Device;

            /* Create our Direct3D texture */
            RenderTargetTexture = new RenderTargetTexture(device, width, height, m_format, optionFlags);

            /* Create the Direct2D render target, using our Direct3D texture we just created */
            D2DRenderTarget = new Direct2DRenderTarget(m_directCanvasFactory.DeviceContext, m_renderTargetTexture.InternalDxgiSurface, m_format);
        }

        /// <summary>
        /// Creates a new instance of the DrawingLayer
        /// </summary>
        /// <param name="directCanvasFactory">The factory that the DrawingLayer belongs to</param>
        /// <param name="renderTargetTexture">The Direct3D texture to use</param>
        internal DrawingLayer(DirectCanvasFactory directCanvasFactory, RenderTargetTexture renderTargetTexture)
        {
            m_directCanvasFactory = directCanvasFactory;
            m_format = renderTargetTexture.Description.Format;

            m_drawStateManagement = new DrawStateManagement();

            /* Set our Direct3D texture */
            RenderTargetTexture = renderTargetTexture;

            /* Create the Direct2D render target, using our Direct3D texture we were passed */
            D2DRenderTarget = new Direct2DRenderTarget(m_directCanvasFactory.DeviceContext,
                                                       m_renderTargetTexture.InternalDxgiSurface, 
                                                       m_format);
        }

        /// <summary>
        /// Creates a new instance of the DrawingLayer.  This is the constructor that 
        /// presenter subclasses will use, making sure they override the D2DRenderTarget and RenderTargetTexture
        /// </summary>
        /// <param name="directCanvasFactory">The factory that the DrawingLayer belongs to</param>
        internal DrawingLayer(DirectCanvasFactory directCanvasFactory)
        {
            m_drawStateManagement = new DrawStateManagement();
            m_directCanvasFactory = directCanvasFactory;
        }

        private void OnEnableImageCopyChanged()
        {
            if (!EnableImageCopy)
            {
                if (SystemMemoryTexture != null)
                {
                    SystemMemoryTexture.Dispose();
                    SystemMemoryTexture = null;
                }

                return;
            }

            if (SystemMemoryTexture != null)
                return;

            EnsureSystemMemoryTexture();
        }

        protected void EnsureSystemMemoryTexture()
        {
            if (SystemMemoryTexture != null)
            {
                SystemMemoryTexture.Dispose();
                SystemMemoryTexture = null;
            }

            if (EnableImageCopy == false)
                return;

            var device = m_directCanvasFactory.DeviceContext.Device;

            var description = RenderTargetTexture.Description;

            SystemMemoryTexture = new StagingTexture(device, description.Width, description.Height, description.Format);
        }

        virtual internal void DrawCall()
        {
            if (DrawCalled != null || WorldDrawCalled != null)
            {
                DrawCalledArgs arg = new DrawCalledArgs(this);

                DrawCalled?.Invoke(this, arg);
                WorldDrawCalled?.Invoke(this, arg);
            }
        }

        virtual internal void ComposeCall()
        {
            if (DrawCalled != null || WorldDrawCalled != null)
            {
                DrawCalledArgs arg = new DrawCalledArgs(this);

                DrawCalled?.Invoke(this, arg);
                WorldDrawCalled?.Invoke(this, arg);
            }
        }

        #endregion Initializer

        #region Compose

        /// <summary>
        /// Begins the composition process.  BeingCompose always needs 
        /// to be eventually followed by an EndCompose.
        /// </summary>
        public void BeginCompose()
        {
            m_directCanvasFactory.BeginCompose(RenderTargetTexture, BlendStateMode.AlphaBlend);
        }

        /// <summary>
        /// Begins the composition process.  BeingCompose always needs 
        /// to be eventually followed by an EndCompose.
        /// </summary>
        /// <param name="blendStateMode">The blend state to use</param>
        public void BeginCompose(BlendStateMode blendStateMode)
        {
            m_directCanvasFactory.BeginCompose(RenderTargetTexture, blendStateMode);
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
        public void ComposeLayer(DrawingLayer layer, ref RectangleF sourceArea, ref RectangleF destinationArea, ref RotationParameters rotationTransform, ref Color4 tint)
        {
            m_directCanvasFactory.Compose(layer, ref sourceArea, ref destinationArea, ref rotationTransform, ref tint);

            ComposeCall();
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
        public void ComposeLayer(DrawingLayer layer, RectangleF sourceArea, RectangleF destinationArea, RotationParameters rotationTransform, Color4 tint)
        {
            m_directCanvasFactory.Compose(layer, sourceArea, destinationArea, rotationTransform, tint);

            ComposeCall();
        }

        /// <summary>
        /// Ends the composition process and flushes any remaining commands.  
        /// A BeginCompose must have been call first.
        /// </summary>
        public void EndCompose()
        {
            m_directCanvasFactory.EndCompose();    
        }

        #endregion Compose

        #region Draw

        ////////////////////////////////
        // state Draw

        /// <summary>
        /// Begins the drawing process.  Eventually the EndDraw should be called
        /// </summary>
        public void BeginDraw()
        {
            m_drawStateManagement.BeginDrawState();
            D2DRenderTarget.InternalRenderTarget.BeginDraw();
        }

        /// <summary>
        /// Ends the drawing process and flushes all queued drawing commands.  This should happen after a BeginDraw call.
        /// </summary>
        public void EndDraw()
        {
            if (IsBeginDraw)
            {
                m_drawStateManagement.EndDrawState();
                D2DRenderTarget.InternalRenderTarget.Flush();
                D2DRenderTarget.InternalRenderTarget.EndDraw();
            }
            else
            {
                Console.WriteLine("EndDraw must called after begin draw");
            }
        }

        /// <summary>
        /// Clears the DrawingLayer with the specified color
        /// </summary>
        /// <param name="color">The color to clear the drawing layer</param>
        public void Clear(Color4 color)
        {
            /* More optimized to clear using D2D interface _IF_ BeginDraw has been called */
            if(m_drawStateManagement.BeganDraw)
                D2DRenderTarget.InternalRenderTarget.Clear(color.InternalColor4);
            else
                /* Use D3D interface to clear */
                RenderTargetTexture.Clear(color);
        }

        /// <summary>
        /// Clears the DrawingLayer
        /// </summary>
        public void Clear()
        {
            Clear(new Color4(0,0,0,0));
        }

        ////////////////////////////////
        //Media Player

        /// <summary>
        /// Draws a MediaPlayer's video on to the DrawingLayer
        /// </summary>
        /// <param name="player">The MediaPlayer to draw</param>
        public void DrawMediaPlayer(MediaPlayer player)
        {
            m_drawStateManagement.DrawPreamble();

            if (player.InternalBitmap == null)
                return;

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(player.InternalBitmap);

            DrawCall();
        }

        /// <summary>
        /// Draws a MediaPlayer's video on to the DrawingLayer
        /// </summary>
        /// <param name="player">The MediaPlayer to draw</param>
        /// <param name="destinationRectangle">The destination rectangle</param>
        /// <param name="opacity">The transparency level, 0 - 1</param>
        public void DrawMediaPlayer(MediaPlayer player, RectangleF destinationRectangle, float opacity)
        {
            m_drawStateManagement.DrawPreamble();

            if (player.InternalBitmap == null)
                return;

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(player.InternalBitmap,
                                                            destinationRectangle.InternalRectangleF,
                                                            opacity,
                                                            InterpolationMode.Linear);

            DrawCall();
        }

        /// <summary>
        /// Draws a MediaPlayer's video on to the DrawingLayer
        /// </summary>
        /// <param name="player">The MediaPlayer to draw</param>
        /// <param name="destinationRectangle">The destination rectangle</param>
        /// <param name="sourceRectangle">The source rectangle</param>
        /// <param name="opacity">The transparency level, 0 - 1</param>
        public void DrawMediaPlayer(MediaPlayer player, RectangleF destinationRectangle, RectangleF sourceRectangle, float opacity)
        {
            m_drawStateManagement.DrawPreamble();

            if (player.InternalBitmap == null)
                return;

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(player.InternalBitmap, 
                                                            destinationRectangle.InternalRectangleF, 
                                                            opacity, 
                                                            InterpolationMode.Linear, 
                                                            sourceRectangle.InternalRectangleF);

            DrawCall();
        }

        ////////////////////////////////
        // Draw Layer

        /// <summary>
        /// Draws a DrawingLayer on to another DrawingLayer
        /// </summary>
        /// <param name="drawingLayer">The DrawingLayer to draw</param>
        /// <param name="destinationRectangle">The destination rectangle</param>
        /// <param name="sourceRectangle">The source rectangle</param>
        /// <param name="opacity">The transparency level, 0 - 1</param>
        public void DrawLayer(DrawingLayer drawingLayer, RectangleF destinationRectangle, RectangleF sourceRectangle, float opacity)
        {
            m_drawStateManagement.DrawPreamble();

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(drawingLayer.D2DRenderTarget.InternalBitmap, 
                                                            destinationRectangle.InternalRectangleF, 
                                                            opacity, 
                                                            InterpolationMode.Linear, 
                                                            sourceRectangle.InternalRectangleF);

            DrawCall();
        }

        /// <summary>
        /// Draws a DrawingLayer on to another DrawingLayer
        /// </summary>
        /// <param name="drawingLayer">The DrawingLayer to draw</param>
        /// <param name="destinationRectangle">The destination rectangle</param>
        /// <param name="opacity">The transparency level, 0 - 1</param>
        public void DrawLayer(DrawingLayer drawingLayer, RectangleF destinationRectangle, float opacity)
        {
            m_drawStateManagement.DrawPreamble();

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(drawingLayer.D2DRenderTarget.InternalBitmap,
                                                            destinationRectangle.InternalRectangleF,
                                                            opacity,
                                                            InterpolationMode.Linear);

            DrawCall();
        }

        /// <summary>
        /// Draws a DrawingLayer on to another DrawingLayer
        /// </summary>
        /// <param name="drawingLayer">The DrawingLayer to draw</param>
        /// <param name="destinationRectangle">The destination rectangle</param>
        public void DrawLayer(DrawingLayer drawingLayer, RectangleF destinationRectangle)
        {
            m_drawStateManagement.DrawPreamble();

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(drawingLayer.D2DRenderTarget.InternalBitmap,
                                                            destinationRectangle.InternalRectangleF,
                                                            1.0f,
                                                            InterpolationMode.Linear);

            DrawCall();
        }

        /// <summary>
        /// Draws a DrawingLayer on to another DrawingLayer
        /// </summary>
        /// <param name="drawingLayer">The DrawingLayer to draw</param>
        public void DrawLayer(DrawingLayer drawingLayer)
        {
            m_drawStateManagement.DrawPreamble();

            D2DRenderTarget.InternalRenderTarget.DrawBitmap(drawingLayer.D2DRenderTarget.InternalBitmap);

            DrawCall();
        }

        ////////////////////////////////
        // Ellipse
        
        /// <summary>
        /// Internal implementation to draw an ellipse
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="ellipse">The ellipse area</param>
        /// <param name="strokeWidth">The width of the stroke</param>
        /// <param name="localTransform">The local transform</param>
        /// <param name="worldTransform">The world transform</param>
        private void DrawEllipse(Brush brush, Pen pen, Shapes.Ellipse ellipse, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            m_drawStateManagement.DrawPreamble();
            
            var bounds = ellipse.GetBounds();
            
            D2DRenderTarget.InternalRenderTarget.Transform = worldTransform;

            if(brush != null)
            {
                if (BrushHelper.PrepareBrush(brush, this, bounds, localTransform, worldTransform))
                {
                    D2DRenderTarget.InternalRenderTarget.FillEllipse(brush.InternalBrush, ellipse.InternalEllipse);
                }
            }

            if (pen != null && pen.Brush != null)
            {
                if (BrushHelper.PrepareBrush(pen.Brush, this, bounds, localTransform, worldTransform))
                {
                    D2DRenderTarget.InternalRenderTarget.DrawEllipse(pen.Brush.InternalBrush, ellipse.InternalEllipse, pen.Thickness);
                }
            }

            DrawCall();
        }

        public void DrawEllipse(Brush brush, Shapes.Ellipse ellipse)
        {
            DrawEllipse(brush, null, ellipse, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        public void DrawEllipse(Brush brush, Pen pen, Shapes.Ellipse ellipse)
        {
            DrawEllipse(brush, pen, ellipse, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        public void DrawEllipse(Brush brush, Shapes.Ellipse ellipse, GeneralTransform transform)
        {
            var matrixTransform = transform.GetTransform();

            DrawEllipse(brush, null, ellipse, Matrix3x2.Identity, matrixTransform);
        }

        public void DrawEllipse(Brush brush, Pen pen, Shapes.Ellipse ellipse, GeneralTransform transform)
        {
            var matrixTransform = transform.GetTransform();

            DrawEllipse(brush, pen, ellipse, Matrix3x2.Identity, matrixTransform);
        }

        ////////////////////////////////
        // Line

        public void DrawLine(Pen pen, PointF start, PointF end)
        {
            DrawLine(pen.Brush, start, end, pen.Thickness, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        private void DrawLine(Brush brush, PointF start, PointF end, float strokeWidth, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            m_drawStateManagement.DrawPreamble();
            
            /* Set our D2D render target to use the transform */
            D2DRenderTarget.InternalRenderTarget.Transform = worldTransform;

            /* Prepare our brush to be used */
            if (BrushHelper.PrepareBrush(brush, this, new RectangleF(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y), Math.Abs(start.X - end.X), Math.Abs(start.Y - end.Y)), localTransform, worldTransform))
            {
                D2DRenderTarget.InternalRenderTarget.DrawLine(brush.InternalBrush, start.InternalPointF, end.InternalPointF, strokeWidth);
            }
            
            DrawCall();
        }

        ////////////////////////////////
        // Rectangle

        private void DrawRectangle(Brush brush, Pen pen, RectangleF rect, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            m_drawStateManagement.DrawPreamble();
            
            D2DRenderTarget.InternalRenderTarget.Transform = worldTransform;

            if(brush != null)
            {
                if (BrushHelper.PrepareBrush(brush, this, rect, localTransform, worldTransform))
                {
                    D2DRenderTarget.InternalRenderTarget.FillRectangle(brush.InternalBrush, rect.InternalRectangleF);
                }
            }

            if (pen != null && pen.Brush != null)
            {
                if (BrushHelper.PrepareBrush(pen.Brush, this, rect, localTransform, worldTransform))
                {
                    D2DRenderTarget.InternalRenderTarget.DrawRectangle(pen.Brush.InternalBrush, rect.InternalRectangleF, pen.Thickness);
                }
            }

            DrawCall();
        }

        public void DrawRectangle(Brush brush, RectangleF rect)
        {
            DrawRectangle(brush, null, rect, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        public void DrawRectangle(Brush brush, Pen pen, RectangleF rect)
        {
            DrawRectangle(brush, pen, rect, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        public void DrawRectangle(Brush brush, RectangleF rect, GeneralTransform transform)
        {
            var matrixTransform = transform.GetTransform();

            DrawRectangle(brush, null, rect, Matrix3x2.Identity, matrixTransform);
        }

        public void DrawRectangle(Brush brush, Pen pen, RectangleF rect, GeneralTransform transform)
        {
            var matrixTransform = transform.GetTransform();

            DrawRectangle(brush, pen, rect, Matrix3x2.Identity, matrixTransform);
        }

        ////////////////////////////////
        // Rounded Rectangle

        private void DrawRoundedRectangle(Brush brush, RoundedRectangleF rectangle, float strokeWidth, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            m_drawStateManagement.DrawPreamble();
            
            /* Set our D2D render target to use the transform */
            D2DRenderTarget.InternalRenderTarget.Transform = worldTransform;

            /* Prepare our brush to be used */
            if (BrushHelper.PrepareBrush(brush, this, rectangle.GetBounds(), localTransform, worldTransform))
            {
                D2DRenderTarget.InternalRenderTarget.DrawRoundedRectangle(brush.InternalBrush, rectangle.InternalRoundedRectangle, strokeWidth);
            }
            
            DrawCall();
        }

        public void DrawRoundedRectangle(Brush brush, RoundedRectangleF rectangle, float strokeWidth)
        {
            DrawRoundedRectangle(brush, rectangle, strokeWidth, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        public void DrawRoundedRectangle(Brush brush, RoundedRectangleF rectangle, float strokeWidth, GeneralTransform transform)
        {
            var localTransform = transform.GetTransform();

            DrawRoundedRectangle(brush, rectangle, strokeWidth, Matrix3x2.Identity, localTransform);
        }

        /// <summary>
        /// Internal implementation to fill a rounded rectangle
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="rectangle">The rounded rectangle area</param>
        /// <param name="localTransform">The local transform</param>
        /// <param name="worldTransform">The world transform</param>
        private void FillRoundedRectangle(Brush brush, RoundedRectangleF rectangle, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            m_drawStateManagement.DrawPreamble();
            
            /* Set our D2D render target to use the transform */
            D2DRenderTarget.InternalRenderTarget.Transform = worldTransform;

            /* Prepare our brush to be used */
            if (BrushHelper.PrepareBrush(brush, this, rectangle.GetBounds(), localTransform, worldTransform))
            {
                D2DRenderTarget.InternalRenderTarget.FillRoundedRectangle(brush.InternalBrush, rectangle.InternalRoundedRectangle);
            }
            
            DrawCall();
        }

        /// <summary>
        /// Fills a rounded rectangle on the DrawingLayer with the given brush
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="rectangle">The rounded rectangle area</param>
        public void FillRoundedRectangle(Brush brush, RoundedRectangleF rectangle)
        {
            FillRoundedRectangle(brush, rectangle, Matrix3x2.Identity, Matrix3x2.Identity);
        }

        /// <summary>
        /// Fills a rounded rectangle on the DrawingLayer with the given brush
        /// </summary>
        /// <param name="brush">The brush to fill</param>
        /// <param name="rectangle">The rounded rectangle area</param>
        /// <param name="transform">The transformation to apply to the rounded rectangle</param>
        public void FillRoundedRectangle(Brush brush, RoundedRectangleF rectangle, GeneralTransform transform)
        {
            var localTransform = transform.GetTransform();

            FillRoundedRectangle(brush, rectangle, Matrix3x2.Identity, localTransform);
        }

        ////////////////////////////////
        // Geometry

        public void DrawGeometry(Brush brush, Shapes.Geometry geometry, float thickness)
        {
            m_drawStateManagement.DrawPreamble();

            geometry.Draw(this, brush, thickness);

            DrawCall();
        }

        public void FillGeometry(Brush brush, Shapes.Geometry geometry)
        {
            m_drawStateManagement.DrawPreamble();

            geometry.Fill(this, brush);
            
            DrawCall();
        }

        ////////////////////////////////
        // Text

        private SlimDX.DirectWrite.Factory m_directWriteFactory;
        public void DrawText(Brush brush, string text, float x, float y)
        {
            DrawText(brush, text, x, y, 12);
        }

        public void DrawText(Brush brush, string text, float x, float y, float fontsize)
        {
            DrawText(brush, text, x, y, fontsize, "Segoe UI");
        }

        public void DrawText(Brush brush, string text, float x, float y, float fontSize, string FontFamily)
        {
            if (m_directWriteFactory == null)
            {
                m_directWriteFactory = new Factory(FactoryType.Isolated);
            }

            using (TextFormat textFormat = new TextFormat(m_directWriteFactory, FontFamily, FontWeight.Bold, FontStyle.Normal, FontStretch.Normal, fontSize, ""))
            {
                DrawText(brush, text, x, y, textFormat);
            }
        }

        private void DrawText(Brush brush, string text, float x, float y, TextFormat format)
        {
            m_drawStateManagement.DrawPreamble();
            
            if (x > Width - 1 || y > Height - 1)
                return;

            var area = new RectangleF(x, y, Width, Height);

            var matrix = Matrix3x2.Identity;

            if (BrushHelper.PrepareBrush(brush, this, area, matrix, Matrix3x2.Identity))
            {
                D2DRenderTarget.InternalRenderTarget.DrawText(text, format, area.InternalRectangleF, brush.InternalBrush);
            }
            
            DrawCall();
        }

        ////////////////////////////////
        // Image

        public void DrawImage(Image sourceImage, Rectangle destRect)
        {
            sourceImage.CopyToDrawingLayer(this, destRect);

            DrawCall();
        }

        public void DrawImage(Image sourceImage)
        {
            sourceImage.CopyToDrawingLayer(this);

            DrawCall();
        }

        ////////////////////////////////
        // Opacity Layer

        /// <summary>
        /// Fills an opacity mask on the DrawingLayer
        /// </summary>
        /// <param name="brush">The brush containing the content to fill</param>
        /// <param name="mask">The opacity mask</param>
        /// <param name="sourceRect">The source rectangle to use</param>
        /// <param name="destinationRect">The destination rectangle to use</param>
        public void FillOpacityMask(Brush brush, DrawingLayer mask, RectangleF destinationRect, RectangleF sourceRect)
        {
            m_drawStateManagement.DrawPreamble();

            /* Prepare our brush to be used */
            if (BrushHelper.PrepareBrush(brush, this, destinationRect, Matrix3x2.Identity, Matrix3x2.Identity))
            {

                D2DRenderTarget.InternalRenderTarget.AntialiasMode = AntialiasMode.Aliased;

                D2DRenderTarget.InternalRenderTarget.FillOpacityMask(mask.D2DRenderTarget.InternalBitmap, brush.InternalBrush,
                                                                     OpacityMaskContent.Graphics,
                                                                     sourceRect.InternalRectangleF,
                                                                     destinationRect.InternalRectangleF);
            }

            DrawCall();
        }

        ////////////////////////////////
        // Effect

        public void ApplyEffect(ShaderEffect effect, DrawingLayer output, bool clearOutput = true)
        {
            m_directCanvasFactory.ApplyEffect(effect, this, output, clearOutput);

            output.DrawCall();

            DrawCall();
        }

        public void ApplyEffect(ShaderEffect effect, DrawingLayer output, Rectangle targetRect, bool clearOutput = true)
        {
            m_directCanvasFactory.ApplyEffect(effect, this, output, targetRect, clearOutput);

            output.DrawCall();

            DrawCall();
        }

        #endregion Draw

        #region Dispose

        ~DrawingLayer()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            //render targets
            if(SystemMemoryTexture != null)
            {
                SystemMemoryTexture.Dispose();
                SystemMemoryTexture = null;
            }

            if(m_d2DRenderTarget != null)
            {
                m_d2DRenderTarget.Dispose();
                m_d2DRenderTarget = null;
            }

            if (m_renderTargetTexture != null)
            {
                m_renderTargetTexture.Dispose();
                m_renderTargetTexture = null;
            }

            //direct write
            if(m_directWriteFactory != null)
            {
                m_directWriteFactory.Dispose();
                m_directWriteFactory = null;
            }
        }

        #endregion Dispose
    }
}
