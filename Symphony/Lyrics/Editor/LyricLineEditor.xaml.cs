using Symphony.Player;
using Symphony.UI;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Symphony.Lyrics
{
    public class LineUpdateArgs : EventArgs
    {
        public LyricLine Line { get; set; }

        public LineUpdateArgs(LyricLine line)
        {
            Line = line;
        }
    }

    /// <summary>
    /// LyricLineEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricLineEditor : Window
    {
        Storyboard PopupOff;
        ShadowWindow shadow;
        DispatcherTimer timer;
        LyricsEditor LyricEditor;
        bool inited = false;
        double DurationView = 500;
        double PositionView = 500;
        double ViewSensitive = 1.5;

        public event EventHandler<LineUpdateArgs> LineUpdated;

        public LyricLine line;

        Lyric Lyric;

        public LyricLineEditor(Lyric Lyric, LyricLine line, LyricsEditor Parent)
        {
            Owner = Parent;
            LyricEditor = Parent;

            shadow = new ShadowWindow(this, null, 12, 1, true, true);
            shadow.Show();

            InitializeComponent();

            this.Lyric = Lyric;

            this.line = line;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;

            layoutEditor.Init(ref this.line);
            blurEditor.Init(ref this.line);
            shadowEditor.Init(ref this.line);

            layoutEditor.LineChanged += LineChanged;
            blurEditor.LineUpdated += LineChanged;
            shadowEditor.LineUpdated += LineChanged;

            UpdateContentEditor();

            Update();

            PopupOff = (Storyboard)FindResource("PopupOff");
            PopupOff.Completed += delegate
            {
                base.Close();
            };
        }

        #region Window

        public new void Close()
        {
            PopupOff.Begin();

            if(grid.CacheMode == null)
            {
                grid.CacheMode = new BitmapCache();
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount > 1)
            {
                if (WindowState == WindowState.Maximized)
                    WindowState = WindowState.Normal;
                else
                    WindowState = WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion Window

        #region Update

        public void UpdateContentEditor()
        {
            EditorGrid.Children.Clear();
            if (line.Content is TextContent)
            {
                TextContentEditor editor = new TextContentEditor();
                editor.Init(line.Content);
                editor.ContentUpdated += Editor_ContentUpdated;

                EditorGrid.Children.Add(editor);
            }
            else if(line.Content is ImageContent)
            {
                ImageContentEditor editor = new ImageContentEditor();
                editor.Init(line.Content);
                editor.ContentUpdated += Editor_ContentUpdated;

                EditorGrid.Children.Add(editor);
            }
            else
            {
                throw new NotImplementedException("Unknown IContent Type");
            }
        }

        bool firstrun = true;
        double lastPosition;
        double lastDuration;
        public void Update()
        {
            inited = false;

            Sld_FadeIn_Length.Value = line.FadeInLength;
            Sld_FadeOutLength.Value = line.FadeOutLength;

            Sld_Duration.Value = line.Duration;
            if (firstrun)
            {
                lastDuration = line.Duration;
                firstrun = false;
            }
            double diffDuration = Math.Abs(line.Duration - lastDuration);
            Sld_Duration.Minimum = Math.Max(-1, Math.Min(line.Duration - diffDuration * ViewSensitive, line.Duration - DurationView * 0.5));
            Sld_Duration.Maximum = Math.Max(line.Duration + diffDuration * ViewSensitive, line.Duration + DurationView * 0.5);
            lastDuration = line.Duration;

            Sld_Position.Value = line.Position;
            if (firstrun)
            {
                lastPosition = line.Position;
                firstrun = false;
            }
            double diff = Math.Abs(lastPosition - line.Position);
            Sld_Position.Minimum = Math.Max(0, Math.Min(line.Position - diff * ViewSensitive, line.Position - PositionView * 0.5));
            Sld_Position.Maximum = Math.Max(line.Position + PositionView * 0.5, line.Position + diff * ViewSensitive);
            lastPosition = line.Position;

            Cbb_FadeIn.SelectedIndex = (int)line.FadeIn + 1;
            Cbb_FadeOut.SelectedIndex = (int)line.FadeOut + 1;
            
            string summaryText = line.EditorText.Replace("\n", "/").Replace("\r", "");
            if(summaryText.Length > 18)
            {
                StringBuilder b = new StringBuilder();
                for (int i = 0; i < 18; i++)
                {
                    b.Append(summaryText[i]);
                }
                b.Append("…");
                summaryText = b.ToString();
            }
            Tb_Title.Text = string.Format("{0} - {1} ({2})", LanguageHelper.FindText("Lang_Lyric_Editor_Line_Title"), line.EditorComment, summaryText);
            Title = Tb_Title.Text;

            inited = true;
        }

        private void LineChanged(object sender, EventArgs e)
        {
            LineUpdated?.Invoke(this, new LineUpdateArgs(line));
        }

        private void Editor_ContentUpdated(object sender, IContentUpdatedArgs e)
        {
            line.Content = e.Content;

            LineUpdated?.Invoke(this, new LineUpdateArgs(line));

            Update();
        }

        #endregion Update

        #region LineDefualt

        private void Sld_FadeOutLength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                line.FadeOutLength = Sld_FadeOutLength.Value;

                LineUpdated?.Invoke(this, new LineUpdateArgs(line));
            }
        }

        private void Cbb_FadeOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                line.FadeOut = (FadeOutMode)(Cbb_FadeOut.SelectedIndex - 1);

                LineUpdated?.Invoke(this, new LineUpdateArgs(line));
            }
        }

        private void Sld_FadeIn_Length_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(inited)
            {
                line.FadeInLength = Sld_FadeIn_Length.Value;

                LineUpdated?.Invoke(this, new LineUpdateArgs(line));
            }
        }

        private void Cbb_FadeIn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                line.FadeIn = (FadeInMode)(Cbb_FadeIn.SelectedIndex - 1);

                LineUpdated?.Invoke(this, new LineUpdateArgs(line));
            }
        }

        private void Sld_Duration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if (e.NewValue > 0)
                    line.Duration = e.NewValue;
                else
                    line.Duration = -1;

                if (timer.IsEnabled)
                    timer.Stop();
                timer.Start();
            }

            if (Tb_Duration != null)
            {
                if (line.Duration < 0)
                {
                    Tb_Duration.Text = LanguageHelper.FindText("Lang_Auto");
                }
                else if(line.Duration < 60000)
                {
                    Tb_Duration.Text = TimeSpan.FromMilliseconds(e.NewValue).ToString(@"ss\:fff");
                }
                else
                {
                    Tb_Duration.Text = TimeSpan.FromMilliseconds(e.NewValue).ToString(@"mm\:ss\:fff");
                }
            }
        }

        private void Sld_Position_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                line.Position = e.NewValue;

                if (timer.IsEnabled)
                    timer.Stop();
                timer.Start();
            }

            if (Tb_Position != null)
            {
                if (line.Position < 60000)
                {
                    Tb_Position.Text = TimeSpan.FromMilliseconds(line.Position).ToString(@"ss\:fff");
                }
                else
                {
                    Tb_Position.Text = TimeSpan.FromMilliseconds(line.Position).ToString(@"mm\:ss\:fff");
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(Mouse.LeftButton != MouseButtonState.Pressed)
            {
                timer.Stop();

                LineUpdated?.Invoke(this, new LineUpdateArgs(line));

                Update();
            }
        }

        private void scrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + e.Delta);
            e.Handled = true;
        }

        private void Bt_FadeIn_KeySpline_Click(object sender, RoutedEventArgs e)
        {
            GraphEditor ge = new GraphEditor(this, line.FadeInKeySpline);
            ge.Updated += delegate (object s, KeySplineUpdatedArgs arg)
            {
                line.FadeInKeySpline = arg.KeySpline;
            };

            ge.ShowDialog();

            LineUpdated?.Invoke(this, new LineUpdateArgs(line));
        }

        private void Bt_FadeOut_KeySpline_Click(object sender, RoutedEventArgs e)
        {
            GraphEditor ge = new GraphEditor(this, line.FadeOutKeySpline);
            ge.Updated += delegate (object s, KeySplineUpdatedArgs arg)
            {
                line.FadeOutKeySpline = arg.KeySpline;
            };

            ge.ShowDialog();

            LineUpdated?.Invoke(this, new LineUpdateArgs(line));
        }

        #endregion LineDefault

        private void Menu_Content_Text_Click(object sender, RoutedEventArgs e)
        {
            DialogMessageResult r = DialogMessage.Show(this, LanguageHelper.FindText("Lang_MakeNewPrompt"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.YesNo);

            if(r == DialogMessageResult.Yes)
            {
                if(line.Content is Util.IRemovable)
                {
                    ((Util.IRemovable)line.Content).Remove();
                }

                if(line.Content is IDisposable)
                {
                    ((IDisposable)line.Content).Dispose();
                }

                Lyric.ResourceGarbageCollection();

                line.Content = LyricEditor.GetNewTextContent();

                UpdateContentEditor();

                Update();

                LineUpdated?.Invoke(this, new LineUpdateArgs(line));
            }
        }

        private void Menu_Content_Image_Click(object sender, RoutedEventArgs e)
        {
            DialogMessageResult r = DialogMessage.Show(this, LanguageHelper.FindText("Lang_MakeNewPrompt"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.YesNo);

            if(r == DialogMessageResult.Yes)
            {
                if (line.Content is Util.IRemovable)
                {
                    ((Util.IRemovable)line.Content).Remove();
                }

                if (line.Content is IDisposable)
                {
                    ((IDisposable)line.Content).Dispose();
                }

                Lyric.ResourceGarbageCollection();

                line.Content = new ImageContent(Lyric);

                UpdateContentEditor();

                Update();

                LineUpdated?.Invoke(this, new LineUpdateArgs(line));
            }
        }
    }
}
