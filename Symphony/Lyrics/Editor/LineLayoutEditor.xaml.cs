using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Symphony.Lyrics
{
    /// <summary>
    /// LineLayoutEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LineLayoutEditor : UserControl
    {
        LyricLine Line;
        bool inited = false;

        public event EventHandler LineChanged;

        public LineLayoutEditor()
        {
            InitializeComponent();
        }

        public void Init(ref LyricLine line)
        {
            Line = line;

            Update();
        }

        public void Update()
        {
            inited = false;

            Ag_Angle.Angle = Line.ContentRotation.Angle;

            Tb_Margin_Bottom.Value = Line.Margin.Bottom;
            Tb_Margin_Top.Value = Line.Margin.Top;
            Tb_Margin_Left.Value = Line.Margin.Left;
            Tb_Margin_Right.Value = Line.Margin.Right;
            Tb_MinSize_Height.Value = Line.MinSize.Height;
            Tb_MinSize_Width.Value = Line.MinSize.Width;
            Tb_Offset_X.Value = Line.BoxOffset.X;
            Tb_Offset_Y.Value = Line.BoxOffset.Y;

            Sld_Opacity.Value = Line.Opacity * 100;

            switch (Line.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    Cbb_HorizontalAlignment.SelectedIndex = 0;
                    break;
                case HorizontalAlignment.Center:
                    Cbb_HorizontalAlignment.SelectedIndex = 1;
                    break;
                case HorizontalAlignment.Right:
                    Cbb_HorizontalAlignment.SelectedIndex = 2;
                    break;
                case HorizontalAlignment.Stretch:
                    Cbb_HorizontalAlignment.SelectedIndex = 3;
                    break;
            }

            switch (Line.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    Cbb_VerticalAlignment.SelectedIndex = 0;
                    break;
                case VerticalAlignment.Center:
                    Cbb_VerticalAlignment.SelectedIndex = 1;
                    break;
                case VerticalAlignment.Bottom:
                    Cbb_VerticalAlignment.SelectedIndex = 2;
                    break;
                case VerticalAlignment.Stretch:
                    Cbb_VerticalAlignment.SelectedIndex = 3;
                    break;
            }

            inited = true;
        }

        private void Ag_Angle_AngleChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                Line.ContentRotation.Angle = e.NewValue;

                LineChanged?.Invoke(this, null);
            }
        }

        private void Tb_Offset_Y_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                Point pt = Line.BoxOffset;
                pt.Y = e.NewValue;
                Line.BoxOffset = pt;

                LineChanged?.Invoke(this, null);
            }
        }

        private void Tb_Offset_X_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                Point pt = Line.BoxOffset;
                pt.X = e.NewValue;
                Line.BoxOffset = pt;

                LineChanged?.Invoke(this, null);
            }
        }

        private void Tb_MinSize_Height_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                Size size = Line.MinSize;
                size.Height = e.NewValue;
                Line.MinSize = size;

                LineChanged?.Invoke(this, null);
            }
        }

        private void Tb_MinSize_Width_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                Size size = Line.MinSize;
                size.Width = e.NewValue;
                Line.MinSize = size;

                LineChanged?.Invoke(this, null);
            }
        }

        private void Tb_Margin_Right_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                Thickness margin = Line.Margin;
                margin.Right = e.NewValue;
                Line.Margin = margin;

                LineChanged?.Invoke(this, null);
            }
        }

        private void Tb_Margin_Left_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                Thickness margin = Line.Margin;
                margin.Left = e.NewValue;
                Line.Margin = margin;

                LineChanged?.Invoke(this, null);
            }
        }

        private void Tb_Margin_Bottom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                Thickness margin = Line.Margin;
                margin.Bottom = e.NewValue;
                Line.Margin = margin;

                LineChanged?.Invoke(this, null);
            }
        }

        private void Tb_Margin_Top_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                Thickness margin = Line.Margin;
                margin.Top = e.NewValue;
                Line.Margin = margin;

                LineChanged?.Invoke(this, null);
            }
        }

        private void Cbb_VerticalAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_VerticalAlignment.SelectedIndex)
                {
                    case 0:
                        Line.VerticalAlignment = VerticalAlignment.Top;
                        LineChanged?.Invoke(this, null);
                        break;
                    case 1:
                        Line.VerticalAlignment = VerticalAlignment.Center;
                        LineChanged?.Invoke(this, null);
                        break;
                    case 2:
                        Line.VerticalAlignment = VerticalAlignment.Bottom;
                        LineChanged?.Invoke(this, null);
                        break;
                    case 3:
                        Line.VerticalAlignment = VerticalAlignment.Stretch;
                        LineChanged?.Invoke(this, null);
                        break;
                }
            }
        }

        private void Cbb_HorizontalAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_HorizontalAlignment.SelectedIndex)
                {
                    case 0:
                        Line.HorizontalAlignment = HorizontalAlignment.Left;
                        LineChanged?.Invoke(this, null);
                        break;
                    case 1:
                        Line.HorizontalAlignment = HorizontalAlignment.Center;
                        LineChanged?.Invoke(this, null);
                        break;
                    case 2:
                        Line.HorizontalAlignment = HorizontalAlignment.Right;
                        LineChanged?.Invoke(this, null);
                        break;
                    case 3:
                        Line.HorizontalAlignment = HorizontalAlignment.Stretch;
                        LineChanged?.Invoke(this, null);
                        break;
                }
            }
        }

        private void Sld_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                Line.Opacity = e.NewValue / 100;

                LineChanged?.Invoke(this, null);
            }
        }
    }
}
