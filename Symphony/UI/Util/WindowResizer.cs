using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Symphony
{
    //소스를 어디서 참고했는지 없어짐.
    public class WindowResizer
    {
        private const int WM_SYSCOMMAND = 0x112;
        private HwndSource hwndSource;
        public string status="";
        Window activeWin;
        public double Margin = 0;
        
        public WindowResizer(Window activeW)
        {
            activeWin = activeW as Window;

            activeWin.SourceInitialized += new EventHandler(InitializeWindowSource);
        }

        public bool resetCursor()
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                activeWin.Cursor = Cursors.Arrow;
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetStatus()
        {
            return status;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        public void dragWindow()
        {
            SendMessage(hwndSource.Handle, 161, 2, 0);
            //activeWin.DragMove();
        }

        private void InitializeWindowSource(object sender, EventArgs e)
        {
            hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            hwndSource.AddHook(new HwndSourceHook(WndProc));
        }

        IntPtr retInt = IntPtr.Zero;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        public enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        private void ResizeWindow(ResizeDirection direction)
        {
            if (hwndSource != null)
                SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        public void resizeWindow()
        {
            switch (status)
            {
                case "top":
                    activeWin.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "bottom":
                    activeWin.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "left":
                    activeWin.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "right":
                    activeWin.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "topLeft":
                    activeWin.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "topRight":
                    activeWin.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "bottomLeft":
                    activeWin.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "bottomRight":
                    activeWin.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }

        public void MouseMove(Point pt, double width, double height)
        {
            if (pt.X >= width - Margin * 2)
            {
                if (pt.Y <= Margin)
                {
                    displayResizeCursor("topRight");
                }
                else if (pt.Y >= height - Margin * 2)
                {
                    displayResizeCursor("bottomRight");
                }
                else
                {
                    displayResizeCursor("right");
                }
            }
            else if (pt.X <= Margin)
            {
                if (pt.Y <= Margin)
                {
                    displayResizeCursor("topLeft");
                }
                else if (pt.Y >= height - Margin * 2)
                {
                    displayResizeCursor("bottomLeft");
                }
                else
                {
                    displayResizeCursor("left");
                }
            }
            else
            {
                if (pt.Y <= Margin)
                {
                    displayResizeCursor("top");
                }
                else if (pt.Y >= height-Margin*2)
                {
                    displayResizeCursor("bottom");
                }
                else
                {
                    resetCursor();
                }
            }
        }

        public void displayResizeCursor(string sender)
        {

            status = sender;

            switch (status)
            {
                case "top":
                    activeWin.Cursor = Cursors.SizeNS;
                    break;
                case "bottom":
                    activeWin.Cursor = Cursors.SizeNS;
                    break;
                case "left":
                    activeWin.Cursor = Cursors.SizeWE;
                    break;
                case "right":
                    activeWin.Cursor = Cursors.SizeWE;
                    break;
                case "topLeft":
                    activeWin.Cursor = Cursors.SizeNWSE;
                    break;
                case "topRight":
                    activeWin.Cursor = Cursors.SizeNESW;
                    break;
                case "bottomLeft":
                    activeWin.Cursor = Cursors.SizeNESW;
                    break;
                case "bottomRight":
                    activeWin.Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    break;
            }
        }
    }
}
