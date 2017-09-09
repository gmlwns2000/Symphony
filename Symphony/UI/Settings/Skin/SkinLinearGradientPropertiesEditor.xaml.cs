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
    /// SkinLinearGradientPropertiesEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkinLinearGradientPropertiesEditor : UserControl
    {
        LinearGradientBrush brush;
        bool inited = false;

        public event EventHandler<ObjectChangedArgs<LinearGradientBrush>> LinearGradientBrushUpdated;

        public SkinLinearGradientPropertiesEditor(LinearGradientBrush brush)
        {
            InitializeComponent();

            this.brush = brush;

            Update();
        }

        public void Update()
        {
            inited = false;

            UpdatePreview();

            Tb_Opacity.Value = brush.Opacity * 100;
            Tb_End_X.Value = brush.EndPoint.X * 100;
            Tb_End_Y.Value = brush.EndPoint.Y * 100;
            Tb_Start_X.Value = brush.StartPoint.X * 100;
            Tb_Start_Y.Value = brush.StartPoint.Y * 100;

            inited = true;
        }

        public void UpdatePreview()
        {
            inited = false;

            Bd_Preview.Background = brush.Clone();
            Bd_Preview.Background.Freeze();

            inited = true;
        }

        private void Tb_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if (e.NewValue < 0 || e.NewValue > 100)
                {
                    Tb_Opacity.Value = Math.Min(100, Math.Max(0, e.NewValue));

                    return;
                }

                brush.Opacity = e.NewValue / 100;

                UpdatePreview();

                LinearGradientBrushUpdated?.Invoke(this, new ObjectChangedArgs<LinearGradientBrush>(brush));
            }
        }

        private void Tb_End_Y_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if(e.NewValue < 0 || e.NewValue > 100)
                {
                    Tb_End_Y.Value = Math.Min(100, Math.Max(0, e.NewValue));

                    return;
                }

                brush.EndPoint = new Point(brush.EndPoint.X, e.NewValue / 100);

                UpdatePreview();

                LinearGradientBrushUpdated?.Invoke(this, new ObjectChangedArgs<LinearGradientBrush>(brush));
            }
        }

        private void Tb_End_X_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if (e.NewValue < 0 || e.NewValue > 100)
                {
                    Tb_End_X.Value = Math.Min(100, Math.Max(0, e.NewValue));

                    return;
                }

                brush.EndPoint = new Point(e.NewValue / 100, brush.EndPoint.Y);

                UpdatePreview();

                LinearGradientBrushUpdated?.Invoke(this, new ObjectChangedArgs<LinearGradientBrush>(brush));
            }
        }

        private void Tb_Start_Y_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if (e.NewValue < 0 || e.NewValue > 100)
                {
                    Tb_Start_Y.Value = Math.Min(100, Math.Max(0, e.NewValue));

                    return;
                }

                brush.StartPoint = new Point(brush.StartPoint.X, e.NewValue / 100);

                UpdatePreview();

                LinearGradientBrushUpdated?.Invoke(this, new ObjectChangedArgs<LinearGradientBrush>(brush));
            }
        }

        private void Tb_Start_X_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if (e.NewValue < 0 || e.NewValue > 100)
                {
                    Tb_Start_X.Value = Math.Min(100, Math.Max(0, e.NewValue));

                    return;
                }

                brush.StartPoint = new Point(e.NewValue / 100, brush.StartPoint.Y);

                UpdatePreview();

                LinearGradientBrushUpdated?.Invoke(this, new ObjectChangedArgs<LinearGradientBrush>(brush));
            }
        }
    }
}
