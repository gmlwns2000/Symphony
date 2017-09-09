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
    /// SkinGradientEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkinGradientEditor : UserControl, IObjectEditor
    {
        GradientBrush brush;
        bool inited = false;

        public event EventHandler<ObjectChangedArgs> ObjectChanged;

        public SkinGradientEditor(GradientBrush brush)
        {
            InitializeComponent();

            this.brush = brush.Clone();

            gradientStop.Init(brush.GradientStops);
            gradientStop.GradientStopCollectionUpdated += GradientStop_GradientStopCollectionUpdated;
            gradientStop.SelectionChanged += GradientStop_SelectionChanged;

            colorEditor.ObjectChanged += ColorEditor_ObjectChanged;
            if (brush.GradientStops.Count > 0)
            {
                gradientStop.SelectedIndex = 0;

                colorEditor.SetColor(brush.GradientStops[gradientStop.SelectedIndex].Color);
            }

            Update();

            UpdatePropertyEditor();
        }

        private void GradientStop_SelectionChanged(object sender, int e)
        {
            if (inited)
            {
                if (gradientStop.SelectedIndex > -1 && gradientStop.SelectedIndex < brush.GradientStops.Count)
                {
                    colorEditor.SetColor(brush.GradientStops[gradientStop.SelectedIndex].Color);
                }
            }
        }

        private void ColorEditor_ObjectChanged(object sender, ObjectChangedArgs e)
        {
            if (inited)
            {
                if (gradientStop.SelectedIndex > -1 && gradientStop.SelectedIndex < brush.GradientStops.Count)
                {
                    brush.GradientStops[gradientStop.SelectedIndex].Color = (Color)e.Object;
                }

                gradientStop.Init(brush.GradientStops);

                UpdatePropertyEditor();

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
            }
        }

        private void GradientStop_GradientStopCollectionUpdated(object sender, ObjectChangedArgs<GradientStopCollection> e)
        {
            if (inited)
            {
                brush.GradientStops = e.Object;

                UpdatePropertyEditor();

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
            }
        }

        public void Update()
        {
            inited = false;

            gradientStop.Init(brush.GradientStops);

            if (gradientStop.SelectedIndex > -1 && gradientStop.SelectedIndex < brush.GradientStops.Count)
            {
                colorEditor.SetColor(brush.GradientStops[gradientStop.SelectedIndex].Color);
            }

            inited = true;
        }

        public void UpdatePropertyEditor()
        {
            gridEditor.Children.Clear();

            if (brush is LinearGradientBrush)
            {
                SkinLinearGradientPropertiesEditor editor = new SkinLinearGradientPropertiesEditor((LinearGradientBrush)brush);

                editor.LinearGradientBrushUpdated += delegate (object obj, ObjectChangedArgs<LinearGradientBrush> arg)
                {
                    brush = arg.Object;

                    Update();

                    ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
                };

                gridEditor.Children.Add(editor);
            }
            else if (brush is RadialGradientBrush)
            {

            }
            else
            {
                Logger.Error("Unknown Brush Type " + brush.ToString());
            }
        }
    }
}
