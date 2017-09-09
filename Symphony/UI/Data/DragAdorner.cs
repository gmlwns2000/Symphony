using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;

namespace Symphony.UI
{
    class DragAdorner : Adorner, IDisposable
    {
        public Rectangle child = null;

        public DragAdorner( UIElement adornedElement, Size size, Brush brush )
            : base( adornedElement )
        {
            Rectangle rect = new Rectangle();
            rect.Fill = brush;
            rect.Width = size.Width;
            rect.Height = size.Height;
            rect.IsHitTestVisible = false;
            child = rect;
        }
  
        public override GeneralTransform GetDesiredTransform( GeneralTransform transform )
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            //result.Children.Add( base.GetDesiredTransform( transform ) );
            result.Children.Add( new TranslateTransform( OffsetLeft, OffsetTop ) );
            return result;
        }

        public static DependencyProperty OffsetLeftProperty = DependencyProperty.Register("OffsetLeft", typeof(double), typeof(DragAdorner), new FrameworkPropertyMetadata(new PropertyChangedCallback(OffsetUpdated)));
        public double OffsetLeft
        {
            get
            {
                return (double)GetValue(OffsetLeftProperty);
            }
            set
            {
                SetValue(OffsetLeftProperty, value);
            }
        }
        
        public static DependencyProperty OffsetTopProperty = DependencyProperty.Register("OffsetTop", typeof(double), typeof(DragAdorner), new FrameworkPropertyMetadata(new PropertyChangedCallback(OffsetUpdated)));
        public double OffsetTop
        {
            get
            {
                return (double)GetValue(OffsetTopProperty);
            }
            set
            {
                SetValue(OffsetTopProperty, value);
            }
        }

        private static void OffsetUpdated(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            DragAdorner s = (DragAdorner)sender;
            s.UpdateLocation();
        }

        protected override Size MeasureOverride( Size constraint )
        {
            child.Measure( constraint );
            return child.DesiredSize;
        }

        protected override Size ArrangeOverride( Size finalSize )
        {
            child.Arrange( new Rect( finalSize ) );
            return finalSize;
        }

        protected override Visual GetVisualChild( int index )
        {
            return child;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }
  
 
        private void UpdateLocation()
        {
            AdornerLayer adornerLayer = Parent as AdornerLayer;
            if( adornerLayer != null )
                adornerLayer.Update( AdornedElement );
        }

        public void Dispose()
        {
            child = null;
        }
    }
}
