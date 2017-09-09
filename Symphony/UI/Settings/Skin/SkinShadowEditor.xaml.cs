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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SkinShadowEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkinShadowEditor : UserControl, IObjectEditor
    {
        DropShadowEffect eff;
        bool init = false;

        public event EventHandler<ObjectChangedArgs> ObjectChanged;

        public SkinShadowEditor(DropShadowEffect effect)
        {
            InitializeComponent();

            eff = effect.Clone();

            Update();
        }

        public void Update()
        {
            init = false;

            Ce_Color.SetColor(eff.Color);

            Tb_Depth.Value = eff.ShadowDepth;
            Tb_Size.Value = eff.BlurRadius;

            Sld_Opacity.Value = eff.Opacity;

            Angle_Angle.Angle = eff.Direction;

            init = true;
        }

        private void Tb_Size_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init)
            {
                eff.BlurRadius = e.NewValue;

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(eff));
            }
        }

        private void Angle_Angle_AngleChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init)
            {
                eff.Direction = e.NewValue;

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(eff));
            }
        }

        private void Tb_Depth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init)
            {
                eff.ShadowDepth = e.NewValue;

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(eff));
            }
        }

        private void Sld_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (init)
            {
                eff.Opacity = e.NewValue;

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(eff));
            }
        }

        private void Ce_Color_ColorUpdated(object sender, ColorUpdatedArgs e)
        {
            if (init)
            {
                eff.Color = e.NewColor;

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(eff));
            }
        }
    }
}
