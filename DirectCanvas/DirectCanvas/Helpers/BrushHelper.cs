using System;
using DirectCanvas.Brushes;
using DirectCanvas.Misc;
using SlimDX;

namespace DirectCanvas.Helpers
{
    class BrushHelper
    {
        static public bool PrepareBrush(Brush brush, DrawingLayer drawingLayer, RectangleF bounds, Matrix3x2 localTransform, Matrix3x2 worldTransform)
        {
            if (brush == null || brush.InternalBrush == null || brush.InternalBrush.Disposed)
                return false;

            var alignment = brush.Alignment;
            var brushSize = brush.BrushSize;
            
            Matrix3x2 currentBrushTransform = Matrix3x2.Identity;
            
            if (brush.Transform != null)
            {
                switch (alignment)
                {
                    case BrushAlignment.DrawingLayerAbsolute:
                        currentBrushTransform = brush.Transform.GetTransform();
                        break;
                    case BrushAlignment.DrawingLayerRelative:
                        currentBrushTransform = brush.Transform.GetTransformRelative(new RectangleF(0,0, drawingLayer.Width, drawingLayer.Height));
                        break;
                    case BrushAlignment.GeometryAbsolute:
                        currentBrushTransform = brush.Transform.GetTransform();
                        break;
                    case BrushAlignment.GeometryRelative:
                        currentBrushTransform = brush.Transform.GetTransformRelative(bounds);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            switch (alignment)
            {
                case BrushAlignment.DrawingLayerAbsolute:
                    brush.InternalBrush.Transform = currentBrushTransform * Matrix3x2.Invert(worldTransform);
                    break;
                case BrushAlignment.DrawingLayerRelative:
                    {
                        var scaleMatrix = Matrix3x2.Scale(drawingLayer.Width / brushSize.Width,
                                                            drawingLayer.Height / brushSize.Height);

                        brush.InternalBrush.Transform = scaleMatrix * currentBrushTransform * Matrix3x2.Invert(worldTransform);
                    }
                    break;
                case BrushAlignment.GeometryAbsolute:
                    {
                        var translate = Matrix3x2.Translation(bounds.InternalRectangleF.Location);
                        brush.InternalBrush.Transform = translate * localTransform * currentBrushTransform;
                    }
                    break;
                case BrushAlignment.GeometryRelative:
                    {
                        var scaleMatrix = Matrix3x2.Scale(bounds.Width / brushSize.Width,
                                                            bounds.Height / brushSize.Height);

                        var translate = Matrix3x2.Translation(bounds.InternalRectangleF.Location);

                        brush.InternalBrush.Transform = scaleMatrix * translate * localTransform * currentBrushTransform;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }
    }
}
