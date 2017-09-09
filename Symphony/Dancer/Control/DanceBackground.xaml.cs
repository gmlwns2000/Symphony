using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NPlayer;

namespace Symphony.Dancer
{
    /// <summary>
    /// DanceBackground.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DanceBackground : Window
    {
        MainWindow mw;
        Ratio Ratio;

        public DanceBackground(Window owner, MainWindow mw, int scrIndex, Ratio ratio)
        {
            InitializeComponent();
            this.mw = mw;
            Owner = owner;
            Ratio = ratio;

            SetScreen(scrIndex);
        }

        public void SetScreen(int index)
        {
            PresentationSource source = PresentationSource.FromVisual(mw);
            double dpiY = source.CompositionTarget.TransformToDevice.M22;

            Screen target;

            if (index < 0 || index > Screen.AllScreens.Length - 1)
            {
                target = Screen.PrimaryScreen;
            }
            else
            {
                target = Screen.AllScreens[index];
            }

            Top = target.Bounds.Y / dpiY;
            Left = target.Bounds.X / dpiY;
            Width = target.Bounds.Width / dpiY;
            Height = target.Bounds.Height / dpiY;

            if(Ratio.Width > Ratio.Height)
            {
                Height = Width / Ratio.Width * Ratio.Height;
                Top += (target.Bounds.Height- Height) * 0.5;
            }
            else
            {
                Width = Height / Ratio.Height * Ratio.Width;
                Left += (target.Bounds.Width - Width) * 0.5;
            }
        }
    }
}
