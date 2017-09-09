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
    /// SkinColorEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkinColorEditor : UserControl, IObjectEditor
    {
        Color color;
        bool inited = false;

        public SkinColorEditor()
        {
            InitializeComponent();

            inited = false;
        }

        public SkinColorEditor(Color color)
        {
            InitializeComponent();

            this.color = color;

            Update();
        }

        public void SetColor(Color color)
        {
            this.color = color;

            Update();
        }

        public Color GetColor()
        {
            if (inited)
                return color;

            return new Color();
        }

        public event EventHandler<ObjectChangedArgs> ObjectChanged;

        public void Update()
        {
            inited = false;

            Ce_Color.SetColor(color);
            Tb_Opacity.Value = color.A / 255 * 100;

            inited = true;
        }

        private void Ce_Color_ColorUpdated(object sender, ColorUpdatedArgs e)
        {
            if (inited)
            {
                color = Color.FromArgb((byte)((Tb_Opacity.Value / 100) * 255), e.NewColor.R, e.NewColor.G, e.NewColor.B);

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(color));
            } 
        }

        private void Tb_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if(e.NewValue > 100 || e.NewValue < 0)
                {
                    Tb_Opacity.Value = Math.Max(0, Math.Min(100, e.NewValue));

                    return;
                }

                color = Color.FromArgb((byte)((e.NewValue / 100) * 255), color.R, color.G, color.B);

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(color));
            }
        }
    }
}
