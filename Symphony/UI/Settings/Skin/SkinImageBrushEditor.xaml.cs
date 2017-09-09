using Microsoft.Win32;
using Symphony.Util;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// SkinImageBrushEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkinImageBrushEditor : UserControl, IObjectEditor
    {
        ImageBrush brush;
        BitmapImage image;
        ThemeHelper parent;
        Resource Resource;

        bool inited = false;

        public event EventHandler<ObjectChangedArgs> ObjectChanged;

        public SkinImageBrushEditor(ImageBrush brush, ThemeHelper parent)
        {
            InitializeComponent();

            this.brush = brush;
            this.parent = parent;
            Resource = new Resource(parent);

            if(brush.ImageSource != null)
            {
                image = (BitmapImage)brush.ImageSource;
                
                if (image.CacheOption != BitmapCacheOption.OnLoad)
                {
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    brush.ImageSource = image;
                }

                Resource.Open(System.IO.Path.GetFileName(image.UriSource.OriginalString));
            }

            Update();
        }

        public void Update()
        {
            inited = false;

            Tb_Opacity.Value = brush.Opacity * 100;

            switch (brush.Stretch)
            {
                case Stretch.None:
                    Cbb_Stretch.SelectedIndex = 0;
                    break;
                case Stretch.Fill:
                    Cbb_Stretch.SelectedIndex = 1;
                    break;
                case Stretch.Uniform:
                    Cbb_Stretch.SelectedIndex = 2;
                    break;
                case Stretch.UniformToFill:
                    Cbb_Stretch.SelectedIndex = 3;
                    break;
            }

            Rect_Preview.Fill = brush.Clone();
            Rect_Preview.Fill.Freeze();

            inited = true;
        }

        OpenFileDialog ofd;
        private void Bt_Open_Click(object sender, RoutedEventArgs e)
        {
            if(ofd == null)
            {
                ofd = new OpenFileDialog();
                ofd.Filter = Util.IO.SupportedImageFilter;
                ofd.Title = LanguageHelper.FindText("Lang_Image_Open");
            }

            if ((bool)ofd.ShowDialog(Window.GetWindow(this)))
            {
                if (File.Exists(ofd.FileName))
                {
                    Resource.Import(ofd.FileName);

                    image = new BitmapImage();
                    image.BeginInit();
                    Uri baseUri = new Uri(parent.DirectoryInfo.FullName+"\\", UriKind.Absolute);
                    image.UriSource = new Uri(baseUri, System.IO.Path.Combine(parent.ResourcesFolderName, Resource.FileName));
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    brush.ImageSource = image;

                    Update();

                    ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
                }
            }
        }

        private void Cbb_Stretch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Stretch.SelectedIndex)
                {
                    case 0:
                        brush.Stretch = Stretch.None;
                        break;
                    case 1:
                        brush.Stretch = Stretch.Fill;
                        break;
                    case 2:
                        brush.Stretch = Stretch.Uniform;
                        break;
                    case 3:
                        brush.Stretch = Stretch.UniformToFill;
                        break;
                }

                Update();

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
            }
        }

        private void Tb_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if(e.NewValue > 100 || e.NewValue < 0)
                {
                    Tb_Opacity.Value = Math.Max(0, Math.Min(e.NewValue, 100));

                    return;
                }

                brush.Opacity = e.NewValue / 100;

                Update();

                ObjectChanged?.Invoke(this, new ObjectChangedArgs(brush));
            }
        }
    }
}
