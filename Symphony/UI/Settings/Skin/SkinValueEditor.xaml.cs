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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SkinValueEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkinValueEditor : UserControl
    {
        string Key;
        ThemeHelper helper;

        public SkinValueEditor(string Key, ThemeHelper helper)
        {
            InitializeComponent();

            this.helper = helper;
            this.Key = Key;

            string name = ThemeHelper.GetName(Key);
            string[] spl = name.Split(new char[1] { '/' }, 2);

            if(spl.Length > 1)
            {
                Tb_Category.Text = spl[0].Trim();
                Tb_Name.Text = spl[1].Trim();
            }
            else
            {
                Tb_Name.Text = name;
            }

            textGrid.ToolTip = Key;
            
            if(helper.Dictionary[Key] is DropShadowEffect)
            {
                SkinShadowEditor editor = new SkinShadowEditor((DropShadowEffect)helper.Dictionary[Key]);

                editor.ObjectChanged += Editor_ObjectChanged;

                grid.Children.Clear();

                grid.Children.Add(editor);
            }
            else if(helper.Dictionary[Key] is Brush)
            {
                SkinBrushEditor brush = new SkinBrushEditor((Brush)helper.Dictionary[Key], helper);

                brush.ObjectChanged += Editor_ObjectChanged;

                grid.Children.Clear();

                grid.Children.Add(brush);
            }
            else if(helper.Dictionary[Key] is Color)
            {
                SkinColorEditor color = new SkinColorEditor((Color)helper.Dictionary[Key]);

                color.ObjectChanged += Editor_ObjectChanged;

                grid.Children.Clear();

                grid.Children.Add(color);
            }
        }

        private void Editor_ObjectChanged(object sender, ObjectChangedArgs e)
        {
            if(e.Object is Freezable)
            {
                Freezable freezeObj = ((Freezable)e.Object).Clone();
                freezeObj.Freeze();

                helper.Dictionary[Key] = freezeObj;
            }
            else
            {
                helper.Dictionary[Key] = e.Object;
            }

            helper.Update();
        }
    }
}
