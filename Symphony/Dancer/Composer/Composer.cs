using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Symphony.Player;
using System.Windows.Threading;
using System.Windows.Input;

namespace Symphony.Dancer
{
    public class Composer
    {
        bool inited = false;

        Window w;
        Plot Plot;
        public bool isEditor = true;
        public event EventHandler<RoutedEventArgs> RendererClosed;
        public int scrIndex = -1;

        public ComposerRender Renderer;

        /// <summary>
        /// 인스턴스를 액터로 재탄생 시킵니다, 그리고 액터들의 수명을 관리합니다.
        /// 였습니다만, 지금은 뭘하는지 당최 모르는 클라스죠이거
        /// </summary>
        public Composer(ref Plot plot, ref PlayerCore np)
        {
            w = new Window();
            w.Top = -100;
            w.Left = -100;
            w.Width = 1;
            w.Height = 1;
            w.WindowStyle = WindowStyle.ToolWindow;
            w.ShowInTaskbar = false;
            w.Show();

            Renderer = new ComposerRender(ref plot, ref np, w, isEditor, scrIndex);
            Renderer.Closed += Renderer_Closed;

            w.Hide();

            Renderer.Show();

            Init(ref plot);
        }

        private void Renderer_Closed(object sender, EventArgs e)
        {
            if (RendererClosed != null)
            {
                RendererClosed(sender, new RoutedEventArgs());
            }
        }

        //TODO: PLAY START NEW PLOT
        public void Init(ref Plot musicPlot)
        {
            Plot = musicPlot;
            inited = true;
        }

        //TODO: CLOSE EVERYTHING
        public void Destroy()
        {
            Renderer.AllowClose = true;
            Renderer.Close();
            Renderer = null;

            w.Close();
        }

        public void GetFocus()
        {
            w.Activate();
            Renderer.Activate();
        }
    }
}
