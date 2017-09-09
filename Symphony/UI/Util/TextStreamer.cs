using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;

namespace Symphony.UI
{
    public class TextStreamer
    {
        public static bool GetAutoTooltip(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoTooltipProperty);
        }

        public static void SetAutoTooltip(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoTooltipProperty, value);
        }

        public static readonly DependencyProperty AutoTooltipProperty = DependencyProperty.RegisterAttached("AutoTooltip",
                typeof(bool), typeof(TextStreamer), new PropertyMetadata(false, OnAutoTooltipPropertyChanged));

        private static void OnAutoTooltipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)d;

            if (textBlock == null) { return; }

            if (e.NewValue.Equals(true))
            {
                textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
                ComputeAutoTooltip(textBlock);
                textBlock.MouseEnter += TextBlock_MouseEnter;
                textBlock.SizeChanged += TextBlock_SizeChanged;
            }
            else
            {
                textBlock.MouseEnter -= TextBlock_MouseEnter;
                textBlock.SizeChanged -= TextBlock_SizeChanged;
            }
        }

        private static void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            ComputeAutoTooltip(sender as TextBlock);
        }

        private static void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ComputeAutoTooltip(sender as TextBlock);
        }

        private static void ComputeAutoTooltip(TextBlock textBlock)
        {
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double width = textBlock.DesiredSize.Width;
            
            if (textBlock.ActualWidth < width)
            {
                textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
                ToolTipService.SetToolTip(textBlock, textBlock.Text);
            }
            else
            {
                textBlock.TextTrimming = TextTrimming.None;
                ToolTipService.SetToolTip(textBlock, null);
            }
        }
    }
}
