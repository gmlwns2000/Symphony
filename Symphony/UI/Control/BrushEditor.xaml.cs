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

namespace Symphony.UI
{
    public class BrushUpdatedArgs
    {
        public Brush NewBrush;

        public BrushUpdatedArgs(Brush NewBrush)
        {
            this.NewBrush = NewBrush;
        }
    }
    /// <summary>
    /// BrushEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BrushEditor : UserControl
    {
        public event EventHandler<BrushUpdatedArgs> BrushUpdated;

        public BrushEditor()
        {
            InitializeComponent();

            colorEditor.ColorUpdated += ColorEditor_ColorUpdated;
        }

        public void SetBrush(Brush br)
        {
            if(br is SolidColorBrush)
            {
                colorEditor.SetColor(((SolidColorBrush)br).Color);
            }
        }

        private void ColorEditor_ColorUpdated(object sender, ColorUpdatedArgs e)
        {
            if (BrushUpdated != null)
            {
                BrushUpdated(this, new BrushUpdatedArgs(new SolidColorBrush(e.NewColor)));
            }
        }
    }
}
