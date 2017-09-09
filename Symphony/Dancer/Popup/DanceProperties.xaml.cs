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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Symphony.Dancer
{
    /// <summary>
    /// DanceProperties.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DanceProperties : Window
    {
        Plot MusicPlot;
        Storyboard PopupOff;
        ComposerRender Renderer;
        Brush borderBrush;
        DispatcherTimer timer = new DispatcherTimer();
        Brush warnBrush;
        bool inited = false;

        public DanceProperties(Window Owner, ref Plot MusicPlot, ref ComposerRender Renderer)
        {
            InitializeComponent();

            this.Owner = Owner;
            this.Renderer = Renderer;
            this.MusicPlot = MusicPlot;

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;

            Tb_Artist.Text = MusicPlot.Metadata.Artist;
            Tb_Album.Text = MusicPlot.Metadata.Album;
            Tb_Author.Text = MusicPlot.Metadata.Author;
            Tb_Title.Text = MusicPlot.Metadata.Title;
            Tb_RatioX.Text = MusicPlot.Ratio.Width.ToString("0");
            Tb_RatioY.Text = MusicPlot.Ratio.Height.ToString("0");

            warnBrush = new SolidColorBrush(Color.FromArgb(180, 255, 30, 50));
            warnBrush.Freeze();
            borderBrush = Tb_Title.BorderBrush;

            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(300);

            switch (MusicPlot.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    Cb_VerticalAlignment.SelectedIndex = 0;
                    break;
                case VerticalAlignment.Center:
                    Cb_VerticalAlignment.SelectedIndex = 1;
                    break;
                case VerticalAlignment.Bottom:
                    Cb_VerticalAlignment.SelectedIndex = 2;
                    break;
                default:
                    throw new NotImplementedException("DP Unknown VerticalAlignment");
            }

            switch (MusicPlot.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    Cb_HorizontalAlignment.SelectedIndex = 0;
                    break;
                case HorizontalAlignment.Center:
                    Cb_HorizontalAlignment.SelectedIndex = 1;
                    break;
                case HorizontalAlignment.Right:
                    Cb_HorizontalAlignment.SelectedIndex = 2;
                    break;
                default:
                    throw new NotImplementedException("DP Unknown VerticalAlignment");
            }

            inited = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                double ratioX = Convert.ToDouble(Tb_RatioX.Text);
                double ratioY = Convert.ToDouble(Tb_RatioY.Text);

                MusicPlot.Ratio = new Ratio((int)ratioX, (int)ratioY);

                Tb_RatioX.Text = MusicPlot.Ratio.Width.ToString("0");
                Tb_RatioY.Text = MusicPlot.Ratio.Height.ToString("0");
                Tb_RatioX.BorderBrush = borderBrush;
                Tb_RatioY.BorderBrush = borderBrush;

                Renderer.Ratio = MusicPlot.Ratio;
                Renderer.SetScreen(Renderer.scrIndex);
            }
            catch
            {
                Tb_RatioX.BorderBrush = warnBrush;
                Tb_RatioY.BorderBrush = warnBrush;
            }
            timer.Stop();
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PopupOff.Begin();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void UpdateRenderer()
        {
            Renderer.Ratio = MusicPlot.Ratio;
            Renderer.VerticalAlignment = MusicPlot.VerticalAlignment;
            Renderer.HorizontalAlignment = MusicPlot.HorizontalAlignment;
            Renderer.SetScreen(Renderer.scrIndex);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cb_VerticalAlignment.SelectedIndex)
                {
                    case 0:
                        MusicPlot.VerticalAlignment = VerticalAlignment.Top;
                        break;
                    case 1:
                        MusicPlot.VerticalAlignment = VerticalAlignment.Center;
                        break;
                    case 2:
                        MusicPlot.VerticalAlignment = VerticalAlignment.Bottom;
                        break;
                    default:
                        break;
                }

                UpdateRenderer();
            }
        }

        private void Cb_HorizontalAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cb_HorizontalAlignment.SelectedIndex)
                {
                    case 0:
                        MusicPlot.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case 1:
                        MusicPlot.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case 2:
                        MusicPlot.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    default:
                        break;
                }

                UpdateRenderer();
            }
        }

        private void Tb_Title_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                MusicPlot.Metadata.Title = Tb_Title.Text;
            }
        }

        private void Tb_Artist_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                MusicPlot.Metadata.Artist = Tb_Artist.Text;
            }
        }

        private void Tb_Album_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                MusicPlot.Metadata.Album = Tb_Album.Text;
            }
        }

        private void Tb_Author_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                MusicPlot.Metadata.Author = Tb_Author.Text;
            }
        }

        private void Tb_RatioX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                if (timer.IsEnabled) { timer.Stop(); }
                timer.Start();
            }
        }

        private void Tb_RatioY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                if (timer.IsEnabled) { timer.Stop(); }
                timer.Start();
            }
        }
    }
}
