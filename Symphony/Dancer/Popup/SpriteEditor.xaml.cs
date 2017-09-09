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
    /// SpriteEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SpriteEditor : Window
    {
        Storyboard PopupOff;
        public Sprite EditedSprite;

        public SpriteEditor(Window Owner, Sprite sprite)
        {
            InitializeComponent();

            this.Owner = Owner;
            this.EditedSprite = sprite;

            switch (sprite.Type)
            {
                case SpriteType.Ellipse:
                    break;
                case SpriteType.Rectangle:
                    break;
                case SpriteType.Triangle:
                    break;
                case SpriteType.Image:

                    break;
                default:
                    break;
            }

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PopupOff.Begin();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            //최적화 옵션
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            Close();
        }

        DispatcherTimer timer;

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            //timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(66);
            //timer.Tick += Timer_Tick;
            //timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                timer.Stop();
                //최적화 옵션
            }
        }
    }
}
