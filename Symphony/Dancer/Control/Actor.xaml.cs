using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using NPlayer;

namespace Symphony.Dancer
{
    /// <summary>
    /// Actor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Actor : Window
    {
        Storyboard Dance;
        nPlayerCore np;
        Plot MusicPlot;
        public bool IsEditor = false;
        public Instance Inst;
        public int index;

        public Actor(bool isEditor, ref nPlayerCore np, ref Plot MusicPlot, Window parent, Instance inst, int index)
        {
            InitializeComponent();
            Owner = parent;
            
            Title = inst.Name;
            IsEditor = isEditor;
            if (IsEditor)
            {
                Lb_Debug.Text = TimeSpan.FromMilliseconds(inst.StartPosition).ToString(@"hh\:mm\:ss") + "/" + inst.StartPosition.ToString("0,0ms/");
            }
            else
            {
                Lb_Debug.Visibility = Visibility.Hidden;
                Lb_Name.Visibility = Visibility.Hidden;
            }
            Inst = inst;
            this.index = index;
            this.np = np;
            this.MusicPlot = MusicPlot;

            Show();
        }

        public void Destory()
        {
            Close();
        }

        private void Grid_Content_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEditor)
            {
                DragMove();
            }
        }

        private void Grid_Content_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsEditor)
            {
                //위치 키프레임 생성; 혹은 수정
            }
        }
    }
}
