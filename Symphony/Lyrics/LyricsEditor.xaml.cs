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
using System.Windows.Shapes;
using Symphony.Player;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Threading;
using Symphony.Util;
using Symphony.Server;

namespace Symphony.Lyrics
{
    /// <summary>
    /// LyricsEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsEditor : Window
    {
        bool inited = false;

        Lyric Lyric;
        Singer Singer;
        PlayerCore np;
        MainWindow mw;
        UI.ShadowWindow shadow;

        BitmapImage Img_Play;
        BitmapImage Img_Pause;
        ImageBrush Brush_Play;
        ImageBrush Brush_Pause;

        List<LyricLineEditor> editors = new List<LyricLineEditor>();

        /// <summary>
        /// 새로만듭니다
        /// </summary>
        public LyricsEditor(ref PlayerCore np, MainWindow mw, MusicMetadata meta)
        {
            shadow = new UI.ShadowWindow(this, mw);
            shadow.Show();

            InitializeComponent();

            this.np = np;

            Lyric = new Lyric(meta);
            LyricHelper.SetWrokingDirectory(Lyric);
            
            inited = true;

            Show();

            WindowState = WindowState.Normal;

            Activate();

            InitEditor();
        }

        /// <summary>
        /// 불러옵니다
        /// </summary>
        public LyricsEditor(ref PlayerCore np, MainWindow mw, string FilePath)
        {
            shadow = new UI.ShadowWindow(this, mw);
            shadow.Show();

            InitializeComponent();

            this.np = np;
            this.mw = mw;

            bool result = LyricLoader.Load(out Lyric, FilePath);

            if (!result)
            {
                UI.DialogMessage.Show(null, LanguageHelper.FindText("Lang_Lyric_Editor_Main_Load_Fail"));
            }
            inited = result;

            Show();

            Activate();

            InitEditor();
        }

        private void InitEditor()
        {
            Closed += LyricsEditor_Closed;

            if (!inited)
            {
                Loaded += LyricsEditor_Loaded;
                return;
            }

            Singer = new Singer(ref Lyric, np, mw, true, true);
            Singer.ResetPosition = false;
            Singer.Optimize = false;
            Singer.VerticalAlignment = VerticalAlignment.Center;
            Singer.HorizontalAlignment = HorizontalAlignment.Center;
            Singer.Show(this);

            np.PlayPaused += Np_PlayPaused;
            np.PlayResumed += Np_PlayResumed;

            Singer.Rendering += Rendering;

            Img_Pause = FindResource("Img_Pause") as BitmapImage;
            Img_Play = FindResource("Img_Play") as BitmapImage;
            Brush_Pause = FindResource("Brush_Pause") as ImageBrush;
            Brush_Play = FindResource("Brush_Play") as ImageBrush;

            Bar_Next.Click += Bar_Next_Click;
            Bar_NextSkip.Click += Bar_NextSkip_Click;
            Bar_Play.Click += Bar_Play_Click;
            Bar_PreSkip.Click += Bar_PreSkip_Click;
            Bar_Previous.Click += Bar_Previous_Click;

            lst_data.ItemsSource = Lyric.Lines;
            lst_data.Items.Refresh();

            UpdatePlay();
        }

        private void LyricsEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (!inited)
            {
                Close();
            }
        }

        #region PlayerControl

        private void Bar_Previous_Click(object sender, EventArgs e)
        {
            Bt_Previous_Click(sender, null);
        }

        private void Bar_PreSkip_Click(object sender, EventArgs e)
        {
            Bt_PreSkip_Click(sender, null);
        }

        private void Bar_Play_Click(object sender, EventArgs e)
        {
            Bt_Play_Click(sender, null);
        }

        private void Bar_NextSkip_Click(object sender, EventArgs e)
        {
            Bt_NextSkip_Click(sender, null);
        }

        private void Bar_Next_Click(object sender, EventArgs e)
        {
            Bt_Next_Click(sender, null);
        }

        private void Bt_Previous_Click(object sender, RoutedEventArgs e)
        {
            np.SetPosition((int)Math.Max(Singer.Position - 750,0));
            Bar_Position.Value = Singer.Position;
            np.Pause(true);
        }

        private void Bt_PreSkip_Click(object sender, RoutedEventArgs e)
        {
            np.SetPosition((int)Math.Max(Singer.Position - 3000,0));
            Bar_Position.Value = Singer.Position;
        }

        private void Bt_Play_Click(object sender, RoutedEventArgs e)
        {
            np.PlayPause();
        }

        private void Bt_NextSkip_Click(object sender, RoutedEventArgs e)
        {
            np.SetPosition((int)Math.Min(Singer.Position + 3000, np.GetLength(TimeUnit.MilliSecond)));
            Bar_Position.Value = Singer.Position;
        }

        private void Bt_Next_Click(object sender, RoutedEventArgs e)
        {
            np.SetPosition((int)Math.Min(Singer.Position + 750, np.GetLength(TimeUnit.MilliSecond)));
            Bar_Position.Value = Singer.Position;
            np.Pause(true);
        }

        private bool moved = false;
        private void Bar_Position_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(np != null && np.isPlay)
            {
                np.SetPosition((int)e.NewValue);
                moved = true;
            }
        }

        private void Rendering(object sender, EventArgs e)
        {
            if (np!=null && np.isPlay && !moved)
            {
                Bar_Position.Maximum = np.GetLength(TimeUnit.MilliSecond);
                Bar_Position.Value = Singer.Position;
            }
            else
            {
                moved = false;
            }
        }

        private void Np_PlayResumed()
        {
            UpdatePlay();
        }

        private void Np_PlayPaused()
        {
            UpdatePlay();
        }

        private void UpdatePlay()
        {
            //TODO
            if (np.isPaused)
            {
                Bar_Play.ImageSource = Img_Play;
                Bt_Play.Background = Brush_Play;
            }
            else
            {
                Bar_Play.ImageSource = Img_Pause;
                Bt_Play.Background = Brush_Pause;
            }
        }

        #endregion PlayerControl

        #region Window

        private void LyricsEditor_Closed(object sender, EventArgs e)
        {
            if (Lyric != null)
            {
                Lyric.ResourceGarbageCollection();
                Lyric.Dispose();
            }

            if (np != null)
            {
                np.Pause(true);
                np.Stop();
                
                np.PlayPaused -= Np_PlayPaused;
                np.PlayResumed -= Np_PlayResumed;
            }

            if (Singer != null)
            {
                Singer.Rendering -= Rendering;
                Singer.Stop();
                Singer.Dispose();
                Singer = null;
            }
        }

        private void Grid_Position_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(Grid_Position);
            if (pt.Y < 40 && pt.X > 0 && pt.X < Grid_Position.ActualWidth && pt.Y < Grid_Position.ActualHeight)
            {
                DragMove();
            }
        }

        #endregion Window

        #region Editor

        double LineEditorWidth = 0;
        double LineEditorHeight = 0;
        private void OpenLineEditor(int index)
        {
            int targetID = Lyric.Lines[index].ID;
            foreach(LyricLineEditor editor in editors)
            {
                if(editor.line.ID == targetID)
                {
                    editor.Activate();

                    return;
                }
            }

            bool ps = np.isPaused;
            np.Pause(true);
            LyricLineEditor le = new LyricLineEditor(Lyric, Lyric.Lines[index], this);
            if (LineEditorHeight > 0)
            {
                le.Width = LineEditorWidth;
                le.Height = LineEditorHeight;
            }

            le.LineUpdated += delegate (object s, LineUpdateArgs arg)
            {
                for(int i = 0; i<Lyric.Lines.Count; i++)
                {
                    if(Lyric.Lines[i].ID == arg.Line.ID)
                    {
                        Lyric.Lines[i] = arg.Line;

                        Lyric.Solt();

                        lst_data.ItemsSource = Lyric.Lines;
                        lst_data.Items.Refresh();

                        Singer.Refresh();

                        break;
                    }
                }
            };

            le.Closed += delegate (object s, EventArgs arg)
            {
                if(!ps)
                    np.Pause(ps);

                Lyric.Solt();

                lst_data.ItemsSource = Lyric.Lines;
                lst_data.Items.Refresh();

                LineEditorHeight = le.ActualHeight;
                LineEditorWidth = le.ActualWidth;

                editors.Remove(le);

                Singer.Refresh();
            };

            le.Show();
            le.Activate();

            editors.Add(le);
        }

        private void Bt_Add_Click(object sender, RoutedEventArgs e)
        {
            int index = Lyric.Add(
                new LyricLine(GetNewTextContent(),
                    new Rotation(), new Point(), new Size(), HorizontalAlignment.Stretch, VerticalAlignment.Stretch, new Thickness(),
                    Singer.Position, -1,
                    FadeInMode.Auto, 0.2, new AnimationKeySpline(0,0,0,1),
                    FadeOutMode.Auto, 0.25, new AnimationKeySpline(0,0,1,0),
                    new LyricBlur(0),
                    new LyricDropShadow(Color.FromArgb(255, 0, 0, 0), 5, 0, 0, 1), 1)
                );

            Singer.Refresh();

            lst_data.ItemsSource = Lyric.Lines;
            lst_data.Items.Refresh();

            OpenLineEditor(index);
        }

        public TextContent GetNewTextContent()
        {
            string text = LanguageHelper.FindText("Lang_Lyric_Editor_Main_Default_Text") + " " + (Lyric.Lines.Count + 1).ToString();
            TextContent content = new TextContent(text, TextAlignment.Center, 26, "NanumBarunGothic", FontStyles.Normal, FontWeights.Normal, Colors.White);
            return content;
        }

        private void Bt_Del_Click(object sender, RoutedEventArgs e)
        {
            while(lst_data.SelectedItems.Count > 0)
            {
                Lyric.RemoveAt(lst_data.SelectedIndex);
                lst_data.ItemsSource = Lyric.Lines;
                lst_data.Items.Refresh();
            }
        }

        private void Bt_Properties_Click(object sender, RoutedEventArgs e)
        {
            MetadataEditor me = new MetadataEditor(new Server.MusicMetadata(Lyric));
            me.Updated += delegate(object s, MetadataUpdated arg)
            {
                Lyric.Metadata.Title = arg.Title;
                Lyric.Metadata.Album = arg.Album;
                Lyric.Metadata.Artist = arg.Artist;
                Lyric.Metadata.FileName = arg.FileName;
                Lyric.Metadata.Author = arg.Author;
            };

            UI.UserControlHostWindow host = new UI.UserControlHostWindow(this, mw, LanguageHelper.FindText("Lang_Lyric_Editor_Metadata_WindowTitle"), me);
            host.ShowDialog();
        }

        SaveFileDialog sfd;
        bool sfd_okay = false;

        private void Bt_Save_Click(object sender, RoutedEventArgs e)
        {
            if(sfd == null)
            {
                sfd = new SaveFileDialog();
                sfd.Title = LanguageHelper.FindText("Lang_Lyric_Save");
                sfd.Filter = LanguageHelper.FindText("Lang_LyricFile") + "|*.lyric";
                sfd.FileOk += Sfd_FileOk;
            }

            sfd_okay = false;

            sfd.ShowDialog(this);

            if (sfd_okay)
            {
                UploadStart();
            }
        }

        private void UploadStart()
        {
            LyricSaver.Save(sfd.FileName, sfd.OverwritePrompt, Lyric);

            UI.DialogMessageResult result = UI.DialogMessage.Show(this, LanguageHelper.FindText("Lang_Lyric_Editor_Main_Confirm_Upload"), LanguageHelper.FindText("Lang_Confirm"), UI.DialogMessageType.YesNo);

            if (result == UI.DialogMessageResult.Yes)
            {
                LyricsUploader uploader = new LyricsUploader(this, sfd.FileName, new MusicMetadata(Lyric));

                uploader.Show();
            }
        }

        private void Sfd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sfd_okay = !e.Cancel;
        }

        private void Bt_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Lst_ItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = lst_data.SelectedIndex;

            OpenLineEditor(index);
        }

        private void Lst_ItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (lst_data.SelectedIndex >= 0 && lst_data.SelectedIndex < Lyric.Lines.Count)
                {
                    np.SetPosition((int)Lyric.Lines[lst_data.SelectedIndex].Position);
                }
            }
        }

        #endregion Editor 

        #region Key

        private void lst_data_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                if(lst_data.SelectedIndex > -1 && lst_data.SelectedIndex < Lyric.Lines.Count)
                {
                    while (lst_data.SelectedItems.Count > 0)
                    {
                        Lyric.RemoveAt(lst_data.SelectedIndex);
                        lst_data.ItemsSource = Lyric.Lines;
                        lst_data.Items.Refresh();
                    }
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Bt_Save_Click(null, null);
            }
            else if(e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Bt_Add_Click(null, null);
            }
            else if(e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Copy();
            }
            else if(e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Paste();
            }
            else if(e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Undo();
            }
            else if(e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                Redo();
            }
        }

        #endregion Key

        #region CopyPasteUndo

        List<LyricLine> lineBuff = new List<LyricLine>();
        private void Copy()
        {
            if(lst_data.SelectedItems != null)
            {
                lineBuff = (List<LyricLine>)lst_data.SelectedItems;
            }
        }

        private void Paste()
        {
            if(lineBuff != null)
            {
                lineBuff = null;
            }
        }

        private void Cut()
        {

        }

        List<Lyric> undoList = new List<Lyric>(50);

        private void saveUndoData()
        {
            if(undoIndex != -1)
            {

            }

            if(undoList.Count >= 50)
            {
                undoList.RemoveAt(0);
            }

            undoList.Add(Lyric);
            undoIndex = undoList.Count - 1;
        }

        int undoIndex = -1;
        private void Undo()
        {
            if(undoList.Count > 0)
            {
                Lyric = undoList[undoList.Count - 1];
            }
        }

        private void Redo()
        {
            if(undoList.Count > 0)
            {
                Lyric = undoList[undoIndex];
            }
        }

        #endregion CopyPasteUndo
    }
}
