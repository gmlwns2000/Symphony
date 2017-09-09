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
    /// LyricBlurEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricBlurEditor : UserControl
    {
        LyricLine Line;
        bool inited;

        public event EventHandler LineUpdated;
        public LyricBlurEditor()
        {
            InitializeComponent();
        }

        public void Init(ref LyricLine Line)
        {
            this.Line = Line;

            Update();

            inited = true;
        }

        public void Update()
        {
            inited = false;

            Sld_Radius.Value = Line.Blur.Radius;

            inited = true;
        }

        private void Sld_Radius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                LyricBlur blur = Line.Blur;
                blur.Radius = e.NewValue;
                Line.Blur = blur;

                LineUpdated?.Invoke(this, null);
            }
        }
    }
}
