using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TextblockVsFormattedtext
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            sw = new Stopwatch();
            sw.Start();

            rnd = new Random();

            grid.IsHitTestVisible = false;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        Random rnd;
        Stopwatch sw;
        double framems = 0;
        int count = 0;
        bool started = false;
        double maxrender = 0;
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            double fr = sw.ElapsedMilliseconds;

            double proc = sw.ElapsedMilliseconds;
            if(count < 10000)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (ftb)
                    {
                        FormattedTextContainer tb = new FormattedTextContainer("Format", rnd.Next(0, (int)grid.ActualWidth), rnd.Next(0, (int)grid.ActualHeight));

                        tb.IsHitTestVisible = false;
                        tb.HorizontalAlignment = HorizontalAlignment.Left;
                        tb.VerticalAlignment = VerticalAlignment.Top;

                        grid.Children.Add(tb);
                        count++;
                    }
                    if (tbb)
                    {
                        TextBlock tb = new TextBlock();

                        tb.IsHitTestVisible = false;
                        tb.HorizontalAlignment = HorizontalAlignment.Left;
                        tb.VerticalAlignment = VerticalAlignment.Top;
                        tb.Text = "TextBlock";
                        tb.Margin = new Thickness(rnd.Next(0, (int)grid.ActualWidth), rnd.Next(0, (int)grid.ActualHeight), 0, 0);

                        grid.Children.Add(tb);
                        count++;
                    }
                    if (glb)
                    {
                        GlyphRunContainer tb = new GlyphRunContainer("GlyphRun", rnd.Next(0, (int)grid.ActualWidth), rnd.Next(0, (int)grid.ActualHeight));

                        tb.IsHitTestVisible = false;
                        tb.HorizontalAlignment = HorizontalAlignment.Left;
                        tb.VerticalAlignment = VerticalAlignment.Top;

                        grid.Children.Add(tb);
                        count++;
                    }
                }

                Counter.Text = count.ToString();
            }
            else
            {
                if (started)
                {
                    Counter.TextAlignment = TextAlignment.Center;
                    Counter.Text = count.ToString() + "\nElapsedTime: " + (sw.ElapsedMilliseconds - startms).ToString() + "ms\nMaxRender: "+ maxrender.ToString()+"ms";
                    started = false;
                }
            }

            maxrender = Math.Max(maxrender, fr - framems - proc + sw.ElapsedMilliseconds);

            FPS.Text = ((int)(1000/(fr - framems))).ToString() + "fps";
            framems = fr;
        }

        bool glb = false;
        bool ftb = false;
        bool tbb = false;
        double startms = 0;

        private void ft_Click(object sender, RoutedEventArgs e)
        {
            ft.IsEnabled = false;
            tb.IsEnabled = false;
            gl.IsEnabled = false;
            glb = false;
            tbb = false;
            ftb = true;
            started = true;
            startms = sw.ElapsedMilliseconds;
            maxrender = 0;
        }

        private void tb_Click(object sender, RoutedEventArgs e)
        {
            ft.IsEnabled = false;
            tb.IsEnabled = false;
            gl.IsEnabled = false;
            glb = false;
            tbb = true;
            ftb = false;
            started = true;
            startms = sw.ElapsedMilliseconds;
            maxrender = 0;
        }

        private void gl_Click(object sender, RoutedEventArgs e)
        {
            ft.IsEnabled = false;
            tb.IsEnabled = false;
            gl.IsEnabled = false;
            glb = true;
            tbb = false;
            ftb = false;
            started = true;
            startms = sw.ElapsedMilliseconds;
            maxrender = 0;
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            grid.Children.Clear();
            ft.IsEnabled = true;
            tb.IsEnabled = true;
            gl.IsEnabled = true;
            started = false;
            glb = false;
            tbb = false;
            ftb = false;
            count = 0;
        }
    }
}
