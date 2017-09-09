using Microsoft.Win32;
using Symphony.Server;
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

namespace Symphony.Lyrics
{
    /// <summary>
    /// ImageContentEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageContentEditor : UserControl, IContentEditor
    {
        ImageContent content;
        bool inited = false;

        public ImageContentEditor()
        {
            InitializeComponent();
        }

        public event EventHandler<IContentUpdatedArgs> ContentUpdated;

        public void Init(IContent content)
        {
            this.content = (ImageContent)content;

            Update();
        }

        bool bitmapRefresh = true;
        public void Update()
        {
            inited = false;

            switch (content.ScalingMode)
            {
                case BitmapScalingMode.Unspecified:
                    Cbb_Resample.SelectedIndex = 0;
                    break;
                case BitmapScalingMode.NearestNeighbor:
                    Cbb_Resample.SelectedIndex = 1;
                    break;
                case BitmapScalingMode.LowQuality:
                    Cbb_Resample.SelectedIndex = 2;
                    break;
                case BitmapScalingMode.HighQuality:
                    Cbb_Resample.SelectedIndex = 3;
                    break;
            }

            switch (content.Stretch)
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

            if (content.Width >= 0)
            {
                Tb_Width.Value = content.Width;
                Tb_Width.IsEnabled = true;
                Tb_Width.Opacity = 1;

                Cb_Width_Auto.IsChecked = false;

                Img.Width = content.Width;
            }
            else
            {
                Tb_Width.Value = 0;
                Tb_Width.IsEnabled = false;
                Tb_Width.Opacity = 0.66;

                Cb_Width_Auto.IsChecked = true;

                Img.Width = double.NaN;
            }
            
            if (content.Height >= 0)
            {
                Tb_Height.Value = content.Height;
                Tb_Height.Opacity = 1;
                Tb_Height.IsEnabled = true;

                Cb_Height_Auto.IsChecked = false;

                Img.Height = content.Height;
            }
            else
            {
                Tb_Height.Value = 0;
                Tb_Height.Opacity = 0.66;
                Tb_Height.IsEnabled = false;

                Cb_Height_Auto.IsChecked = true;

                Img.Height = double.NaN;
            }

            if (bitmapRefresh && content.Resource.IsExist)
            {
                BitmapImage bit = new BitmapImage();
                bit.BeginInit();
                bit.UriSource = new Uri(content.Resource.FilePath);
                bit.CacheOption = BitmapCacheOption.OnLoad;
                bit.EndInit();

                Img.Source = bit;

                Img.Stretch = content.Stretch;

                RenderOptions.SetBitmapScalingMode(Img, content.ScalingMode);

                bit.Freeze();
            }

            inited = true;
        }

        private void Bt_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {

            }
        }

        private void Bt_Open_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = LanguageHelper.FindText("Lang_Image_Open");
                ofd.Filter = Util.IO.SupportedImageFilter;

                if ((bool)ofd.ShowDialog(Window.GetWindow(this)))
                {
                    QueryResult result = content.Resource.Import(ofd.FileName);

                    if (result.Success)
                    {
                        bitmapRefresh = true;

                        Update();

                        content.Resource.Parent.ResourceGarbageCollection();

                        ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
                    }
                    else
                    {
                        UI.DialogMessage.Show(Window.GetWindow(this), result.Message);
                    }
                }
            }
        }

        private void Cb_Width_Auto_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                content.Width = -1;

                Update();

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Cb_Width_Auto_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                if(Tb_Width.Value > 0)
                {
                    content.Width = Tb_Width.Value;
                }
                else
                {
                    content.Width = Img.ActualWidth;
                }

                Update();

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Tb_Width_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited && !(bool)Cb_Width_Auto.IsChecked)
            {
                if (e.NewValue < 0)
                {
                    Tb_Width.Value = Math.Abs(e.NewValue);

                    return;
                }

                Img.Width = e.NewValue;

                content.Width = e.NewValue;

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Cb_Height_Auto_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                content.Height = -1;

                Update();

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Cb_Height_Auto_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                if(Tb_Height.Value > 0)
                {
                    content.Height = Tb_Height.Value;
                }
                else
                {
                    content.Height = Img.ActualHeight;
                }

                Update();

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Tb_Height_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited && !(bool)Cb_Height_Auto.IsChecked)
            {
                if (e.NewValue < 0)
                {
                    Tb_Height.Value = Math.Abs(e.NewValue);

                    return;
                }

                Img.Height = e.NewValue;

                content.Height = e.NewValue;

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Cbb_Stretch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Stretch.SelectedIndex)
                {
                    case 0:
                        content.Stretch = Stretch.None;
                        break;
                    case 1:
                        content.Stretch = Stretch.Fill;
                        break;
                    case 2:
                        content.Stretch = Stretch.Uniform;
                        break;
                    case 3:
                        content.Stretch = Stretch.UniformToFill;
                        break;
                }

                Update();

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Cbb_Resample_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Resample.SelectedIndex)
                {
                    case 0:
                        content.ScalingMode = BitmapScalingMode.Unspecified;
                        break;
                    case 1:
                        content.ScalingMode = BitmapScalingMode.NearestNeighbor;
                        break;
                    case 2:
                        content.ScalingMode = BitmapScalingMode.LowQuality;
                        break;
                    case 3:
                        content.ScalingMode = BitmapScalingMode.HighQuality;
                        break;
                }

                Update();

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }
    }
}
