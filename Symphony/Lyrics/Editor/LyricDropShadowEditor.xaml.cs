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
    /// LyricDropShadowEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricDropShadowEditor : UserControl
    {
        public LyricDropShadowEditor()
        {
            InitializeComponent();
        }

        LyricLine Line;
        bool inited = false;
        public event EventHandler LineUpdated;
        public void Init(ref LyricLine Line)
        {
            this.Line = Line;

            Update();

            inited = true;
        }

        public void Update()
        {
            inited = false;

            An_Shadow_Angle.Angle = Line.Shadow.Direction;

            Sld_Depth.Value = Line.Shadow.Depth;
            Sld_Opacity.Value = Line.Shadow.Opacity * 100;
            Sld_Radius.Value = Line.Shadow.Radius;

            colorEditor.SetColor(Line.Shadow.Color);

            inited = true;
        }

        private void An_Shadow_Angle_AngleChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                LyricDropShadow shadow = Line.Shadow;
                shadow.Direction = e.NewValue;
                Line.Shadow = shadow;

                LineUpdated?.Invoke(this, null);
            }
        }

        private void Sld_Depth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                LyricDropShadow shadow = Line.Shadow;
                shadow.Depth = e.NewValue;
                Line.Shadow = shadow;

                LineUpdated?.Invoke(this, null);
            }
        }

        private void Sld_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                LyricDropShadow shadow = Line.Shadow;
                shadow.Opacity = e.NewValue / 100;
                Line.Shadow = shadow;

                LineUpdated?.Invoke(this, null);
            }
        }

        private void Sld_Radius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                LyricDropShadow shadow = Line.Shadow;
                shadow.Radius = e.NewValue;
                Line.Shadow = shadow;

                LineUpdated?.Invoke(this, null);
            }
        }

        private void colorEditor_ColorUpdated(object sender, UI.ColorUpdatedArgs e)
        {
            if (inited)
            {
                LyricDropShadow shadow = Line.Shadow;
                shadow.Color = e.NewColor;
                Line.Shadow = shadow;

                LineUpdated?.Invoke(this, null);
            }
        }
    }
}
