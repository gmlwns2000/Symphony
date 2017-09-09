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
using Symphony.Player;

namespace Symphony.UI
{
    /// <summary>
    /// PlaymodeSelector.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PlaymodeSelector : UserControl
    {
        public event EventHandler<PlaylistOrder> OrderChanged;

        ImageBrush Brush_Once;
        ImageBrush Brush_Repeat;
        ImageBrush Brush_Random;
        ImageBrush Brush_RepeatOne;

        BitmapImage Img_Order_Once;
        BitmapImage Img_Order_Repeat;
        BitmapImage Img_Order_Random;
        BitmapImage Img_Order_RepeatOne;

        private PlaylistOrder _playlistOrder = PlaylistOrder.Once;
        public PlaylistOrder PlaylistOrder
        {
            get
            {
                return _playlistOrder;
            }
            set
            {
                if (_playlistOrder != value)
                {
                    _playlistOrder = value;

                    Update();

                    OrderChanged?.Invoke(this, value);
                }
            }
        }

        public PlaymodeSelector()
        {
            InitializeComponent();

            Img_Order_Once = FindResource("Img_Order_Once") as BitmapImage;
            Img_Order_Repeat = FindResource("Img_Order_Repeat") as BitmapImage;
            Img_Order_Random = FindResource("Img_Order_Random") as BitmapImage;
            Img_Order_RepeatOne = FindResource("Img_Order_RepeatOne") as BitmapImage;

            Brush_Once = new ImageBrush(Img_Order_Once);
            Brush_Repeat = new ImageBrush(Img_Order_Repeat);
            Brush_Random = new ImageBrush(Img_Order_Random);
            Brush_RepeatOne = new ImageBrush(Img_Order_RepeatOne);

            Brush_Repeat.Freeze();
            Brush_Random.Freeze();
            Brush_Once.Freeze();
            Brush_RepeatOne.Freeze();

            Img_Order_Once.Freeze();
            Img_Order_Random.Freeze();
            Img_Order_Repeat.Freeze();
            Img_Order_RepeatOne.Freeze();

            Update();
        }

        private void Update()
        {
            switch (PlaylistOrder)
            {
                case PlaylistOrder.Once:
                    Bt.Background = Brush_Once;
                    break;
                case PlaylistOrder.Random:
                    Bt.Background = Brush_Random;
                    break;
                case PlaylistOrder.Repeat:
                    Bt.Background = Brush_Repeat;
                    break;
                case PlaylistOrder.RepeatOne:
                    Bt.Background = Brush_RepeatOne;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Next()
        {
            switch (PlaylistOrder)
            {
                case PlaylistOrder.Once:
                    PlaylistOrder = PlaylistOrder.Repeat;
                    break;
                case PlaylistOrder.Random:
                    PlaylistOrder = PlaylistOrder.RepeatOne;
                    break;
                case PlaylistOrder.Repeat:
                    PlaylistOrder = PlaylistOrder.Random;
                    break;
                case PlaylistOrder.RepeatOne:
                    PlaylistOrder = PlaylistOrder.Once;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Bt_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }
    }
}
