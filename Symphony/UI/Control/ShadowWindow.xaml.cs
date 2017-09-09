using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Symphony.UI
{
    /// <summary>
    /// ShadowWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShadowWindow : Window
    {
        public static Thickness MaximizeMargin = new Thickness(6);

        public new double Margin = 12;

        public Window ParentWnd;
        public WindowResizer wr;
        public WindowResizer ownerWr;

        public event EventHandler<MouseLeftDownArgs> MouseLeftButtonDown_Custom;
        public bool IsOnResizing { get; set; }
        public bool IgnoreFreezeMainWindow { get; set; } = false;
        public bool AllowResizing { get; set; }

        public bool parentopen = true;

        MainWindow mw;

        /// <summary>
        /// Shadow window class.
        /// <para/>Usage ===
        /// <para/>ShadowWindow sw = new ShadowWindow(this, mw);
        /// <para/>sw.Show();
        /// <para/>
        /// </summary>
        /// <param name="window">Set Parent Window.</param>
        /// <param name="mw">Set MainWindow. Can use Optimization. Can nullable.</param>
        /// <param name="Margin">Margin. Default = 12</param>
        /// <param name="Opacity">Opacity Default = 1</param>
        /// <param name="allowResize">AllowResize Default = True</param>
        public ShadowWindow(Window window, MainWindow mw, double Margin = 12, double Opacity = 1, bool allowResize = true, bool closeWhenClosing = true)
        {
            Top = -1000;
            Left = -1000;

            this.Margin = Margin;
            this.Opacity = Opacity;
            this.mw = mw;
            AllowResizing = allowResize;

            ParentWnd = window;
            ParentWnd.Loaded += ParentWnd_Loaded;
            ParentWnd.Activated += Parent_Activated;
            ParentWnd.StateChanged += ParentWnd_StateChanged;
            if (closeWhenClosing)
            {
                ParentWnd.Closing += delegate
                {
                    parentopen = false;
                    Close();
                };
            }
            else
            {
                ParentWnd.Closed += delegate
                {
                    parentopen = false;
                    Close();
                };
            }

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            Closed += delegate
            {
                if (parentopen)
                {
                    ParentWnd.Close();
                }

                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            };

            InitializeComponent();
            
            wr = new WindowResizer(this);
            ownerWr = new WindowResizer(ParentWnd);
            
            Activated += ShadowWindow_Activated;
            PreviewMouseMove += ShadowWindow_PreviewMouseMove;
            
            Left = ParentWnd.Left - Margin;
            Top = ParentWnd.Top - Margin;
            Width = ParentWnd.ActualWidth + Margin * 2;
            Height = ParentWnd.ActualHeight + Margin * 2;

            UpdateClip(Width, Height);

            IsOnResizing = false;
        }

        #region Update

        private bool _allowUpdate = true;
        public bool AllowUpdate
        {
            get
            {
                return _allowUpdate;
            }
            set
            {
                if (value)
                {
                    Visibility = Visibility.Visible;
                    Update();
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }
                _allowUpdate = value;
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (ParentWnd.WindowState != WindowState.Maximized && ParentWnd.WindowState != WindowState.Minimized)
            {
                Update();
            }
        }

        private void ParentWnd_StateChanged(object sender, EventArgs e)
        {
            Update(true);
        }

        public void Update(bool force = false)
        {
            if(ParentWnd.WindowState == WindowState.Maximized || ParentWnd.WindowState == WindowState.Minimized || !AllowUpdate)
            {
                Visibility = Visibility.Hidden;
                return;
            }
            else
            {
                Visibility = Visibility.Visible;
            }

            if (!force && !AllowUpdate)
                return;
            
            bool edit = false;

            if (force || Width != ParentWnd.ActualWidth + 2 * Margin)
            {
                edit = true;
            }
            if (force || Height != ParentWnd.ActualHeight + 2 * Margin)
            {
                edit = true;
            }

            Left = ParentWnd.Left - Margin;
            Top = ParentWnd.Top - Margin;
            Width = ParentWnd.ActualWidth + 2 * Margin;
            Height = ParentWnd.ActualHeight + 2 * Margin;
            
            if (force || (edit && !IsOnResizing))
            {
                UpdateClip(Width, Height);
            }
        }

        private void UpdateClip(double width, double height)
        {
            RectangleGeometry geo1 = new RectangleGeometry(new Rect(0, 0, width, height));
            RectangleGeometry geo2 = new RectangleGeometry(new Rect(Margin, Margin, width - 2 * Margin, height - 2 * Margin));

            geo1.Freeze();
            geo2.Freeze();

            CombinedGeometry cgeo = new CombinedGeometry(GeometryCombineMode.Xor, geo1, geo2);
            cgeo.Freeze();

            grid.Clip = cgeo;
        }

        #endregion Update

        #region Activation

        private void ShadowWindow_Activated(object sender, EventArgs e)
        {
            if(AllowResizing)
                ShadowWindow_MouseDown();

            ParentWnd.Activate();
        }

        bool shadowWindowOn = false;

        private void Parent_Activated(object sender, EventArgs e)
        {
            
        }

        private void ParentWnd_Loaded(object sender, RoutedEventArgs e)
        {
            if (!shadowWindowOn)
            {
                shadowWindowOn = true;

                Owner = ParentWnd;

                Show();
            }
        }

        #endregion Activation

        #region Mouse Handle

        private void ShadowWindow_MouseDown()
        {
            Point pt = Mouse.GetPosition(this);

            if ((pt.X < Margin || pt.Y < Margin || pt.X > ActualWidth - Margin || pt.Y > ActualHeight - Margin) && (pt.X >= 0 && pt.Y >= 0 && pt.X <= ActualWidth && pt.Y <= ActualHeight))
            {
                ownerWr.status = wr.status;

                if (mouseLeftbuttondown == null)
                {
                    mouseLeftbuttondown = new DispatcherTimer();
                    mouseLeftbuttondown.Interval = TimeSpan.FromMilliseconds(500);
                    mouseLeftbuttondown.Tick += MouseLeftbuttondown_Tick;
                }

                if (mouseLeftbuttondown.IsEnabled)
                {
                    mouseLeftbuttondown.Stop();
                }
                mouseLeftbuttondownCount++;

                MouseLeftButtonDown_Custom?.Invoke(this, new MouseLeftDownArgs(mouseLeftbuttondownCount));

                mouseLeftbuttondown.Start();

                if (mouseLeftbuttondownCount == 1)
                {
                    IsOnResizing = true;

                    grid.Visibility = Visibility.Hidden;
                    Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
                    if(mw!=null && !IgnoreFreezeMainWindow)
                        mw.StopRenderingWhileClicking();

                    ownerWr.resizeWindow();

                    Update(true);
                    Background = new SolidColorBrush(Colors.Transparent);
                    grid.Visibility = Visibility.Visible;

                    IsOnResizing = false;
                }

                ParentWnd.Cursor = Cursors.Arrow;
            }
        }
        
        int mouseLeftbuttondownCount = 0;
        DispatcherTimer mouseLeftbuttondown;
        private void MouseLeftbuttondown_Tick(object sender, EventArgs e)
        {
            mouseLeftbuttondownCount = 0;
            mouseLeftbuttondown.Stop();
        }
        
        private void ShadowWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(this);

            if(AllowResizing)
                wr.MouseMove(new Point(pt.X - Margin, pt.Y - Margin), ActualWidth - Margin * 2, ActualHeight - Margin * 2);
        }

        public new void DragMove()
        {
            mw.StopRenderingWhileClicking();

            ParentWnd.DragMove();
        }

        #endregion Mouse Handle
    }

    public class MouseLeftDownArgs : EventArgs
    {
        public int ClickCount
        {
            get;
            private set;
        }

        public MouseLeftDownArgs(int clickcount)
        {
            ClickCount = clickcount;
        }
    }
}
