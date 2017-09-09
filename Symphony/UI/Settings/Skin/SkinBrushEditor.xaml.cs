using Symphony.Util;
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
    /// SkinBrushEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkinBrushEditor : UserControl, IObjectEditor
    {
        ThemeHelper parent;
        Brush brush;
        bool init = false;

        public event EventHandler<ObjectChangedArgs> ObjectChanged;

        public SkinBrushEditor(Brush brush, ThemeHelper parent)
        {
            InitializeComponent();

            this.brush = brush.Clone();
            this.parent = parent;

            Update();
        }

        public void Update()
        {
            init = false;

            Bt_Solid.Opacity = 0.33;
            Bt_Gradient.Opacity = 0.33;
            Bt_Image.Opacity = 0.33;

            editorGrid.Children.Clear();

            if (brush is SolidColorBrush)
            {
                Bt_Solid.Opacity = 1;

                SkinSolidColorBrushEditor editor = new SkinSolidColorBrushEditor((SolidColorBrush)brush);
                editor.ObjectChanged += Editor_ObjectChanged;

                editorGrid.Children.Add(editor);
            }
            else if (brush is GradientBrush)
            {
                Bt_Gradient.Opacity = 1;

                SkinGradientEditor editor = new SkinGradientEditor((GradientBrush)brush);
                editor.ObjectChanged += Editor_ObjectChanged;

                editorGrid.Children.Add(editor);
            }
            else if(brush is ImageBrush)
            {
                Bt_Image.Opacity = 1;

                SkinImageBrushEditor editor = new SkinImageBrushEditor((ImageBrush)brush, parent);
                editor.ObjectChanged += Editor_ObjectChanged;

                editorGrid.Children.Add(editor);
            }

            init = true;
        }

        private void Editor_ObjectChanged(object sender, ObjectChangedArgs e)
        {
            if (init)
            {
                brush = (Brush)e.Object;

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
            }
        }

        private void Bt_Solid_Click(object sender, RoutedEventArgs e)
        {
            if (init)
            {
                if (brush is SolidColorBrush)
                    return;

                brush = new SolidColorBrush(RussianRullet.RandomColor());

                Update();

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
            }
        }

        private void Bt_Gradient_Click(object sender, RoutedEventArgs e)
        {
            if (init)
            {
                if (brush is GradientBrush)
                    return;

                brush = new LinearGradientBrush(RussianRullet.RandomColor(), RussianRullet.RandomColor(), RussianRullet.NewRandom.Next(0, 90));

                Update();

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
            }
        }

        private void Bt_Image_Click(object sender, RoutedEventArgs e)
        {
            if (init)
            {
                if (brush is ImageBrush)
                    return;

                brush = new ImageBrush();

                Update();

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
            }
        }
    }
}
