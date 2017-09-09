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

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SkinSolidColorBrushEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkinSolidColorBrushEditor : UserControl, IObjectEditor
    {
        SolidColorBrush brush;
        bool init = false;

        public SkinSolidColorBrushEditor(SolidColorBrush brush)
        {
            InitializeComponent();

            this.brush = brush;

            Update();
        }

        public void Update()
        {
            init = false;

            Ce_Color.SetColor(Color.FromRgb(brush.Color.R, brush.Color.G, brush.Color.B));

            Tb_Opacity.Value = (double)brush.Color.A / 255 * 100;

            init = true;
        }

        public event EventHandler<ObjectChangedArgs> ObjectChanged;

        private void Tb_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init)
            {
                if(e.NewValue > 100)
                {
                    Tb_Opacity.Value = 100;
                }
                else if(e.NewValue < 0)
                {
                    Tb_Opacity.Value = 0;
                }
                else
                {
                    brush.Color = Color.FromArgb((byte)(e.NewValue / 100 * 255), brush.Color.R, brush.Color.G, brush.Color.B);

                    ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
                }
            }
        }

        private void Ce_Color_ColorUpdated(object sender, ColorUpdatedArgs e)
        {
            if (init)
            {
                brush.Color = Color.FromArgb((byte)(Tb_Opacity.Value / 100 * 255), e.NewColor.R, e.NewColor.G, e.NewColor.B);

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
            }
        }
    }
}
