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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Symphony.Lyrics
{
    /// <summary>
    /// TextContentEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TextContentEditor : UserControl, IContentEditor
    {
        static List<ComboBoxItem> items;

        TextContent content;
        bool inited = false;

        public event EventHandler<IContentUpdatedArgs> ContentUpdated;

        public TextContentEditor()
        {
            InitializeComponent();

            if (items == null)
            {
                items = new List<ComboBoxItem>();

                items.Add(makeItem(new FontFamily(TextContent.DefaultFontFamily)));
                items[0].Tag = "Auto";
                items[0].Content = LanguageHelper.FindText("Lang_Auto") + " (" + TextContent.DefaultFontFamily + ")";

                List<FontFamily> fonts = Fonts.SystemFontFamilies.ToList();

                foreach (FontFamily font in fonts)
                {
                    items.Add(makeItem(font));
                }
            }

            Cbb_FontFamily.ItemsSource = items;
            Cbb_FontFamily.Items.Refresh();
        }

        private ComboBoxItem makeItem(FontFamily font)
        {
            ComboBoxItem item = new ComboBoxItem();
            item.FontFamily = font;
            item.FontSize = 12;
            item.Height = 24;
            item.Tag = font.Source;
            item.Content = font.Source;

            return item;
        }

        public void Init(IContent content)
        {
            this.content = (TextContent)content;

            Update();
        }

        public void Update()
        {
            inited = false;

            Ce_Color.SetColor(content.Foreground);

            switch (content.TextAlignment)
            {
                case TextAlignment.Left:
                    Cbb_Alignment.SelectedIndex = 0;
                    break;
                case TextAlignment.Center:
                    Cbb_Alignment.SelectedIndex = 1;
                    break;
                case TextAlignment.Right:
                    Cbb_Alignment.SelectedIndex = 2;
                    break;
                case TextAlignment.Justify:
                    Cbb_Alignment.SelectedIndex = 3;
                    break;
            }

            if(content.FontStyle == FontStyles.Italic)
            {
                Cb_Itelic.IsChecked = true;
            }
            else
            {
                Cb_Itelic.IsChecked = false;
            }

            if(content.FontWeight == FontWeights.Bold)
            {
                Cb_Bold.IsChecked = true;
            }
            else
            {
                Cb_Bold.IsChecked = false;
            }

            Tb_Content.Text = content.Text;

            Tb_Size.Value = content.FontSize;

            for (int i = 0; i < items.Count; i++)
            {
                if((string)items[i].Tag == content.FontFamily)
                {
                    Cbb_FontFamily.SelectedIndex = i;
                    break;
                }
            }

            inited = true;
        }

        private void Ce_Color_ColorUpdated(object sender, UI.ColorUpdatedArgs e)
        {
            if (inited)
            {
                content.Foreground = e.NewColor;

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Cbb_Alignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Alignment.SelectedIndex)
                {
                    case 0:
                        content.TextAlignment = TextAlignment.Left;
                        break;
                    case 1:
                        content.TextAlignment = TextAlignment.Center;
                        break;
                    case 2:
                        content.TextAlignment = TextAlignment.Right;
                        break;
                    case 3:
                        content.TextAlignment = TextAlignment.Justify;
                        break;
                }

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Cb_Itelic_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                content.FontStyle = FontStyles.Italic;

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Cb_Itelic_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                content.FontStyle = FontStyles.Normal;

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Cb_Bold_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                content.FontWeight = FontWeights.Bold;

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Cb_Bold_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                content.FontWeight = FontWeights.Normal;

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Tb_Size_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                content.FontSize = e.NewValue;

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        private void Cbb_FontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                content.FontFamily = (string)((ComboBoxItem)Cbb_FontFamily.SelectedItem).Tag;

                ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));
            }
        }

        DispatcherTimer textTimer;
        private void Tb_Content_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                content.Text = Tb_Content.Text;

                if(textTimer == null)
                {
                    textTimer = new DispatcherTimer();
                    textTimer.Tick += delegate (object s, EventArgs arg)
                    {
                        ContentUpdated?.Invoke(this, new IContentUpdatedArgs(content));

                        textTimer.Stop();
                    };
                    textTimer.Interval = TimeSpan.FromMilliseconds(300);
                }

                if (textTimer.IsEnabled)
                    textTimer.Stop();

                textTimer.Start();
            }
        }

        bool colorOpen = false;
        private void Tb_Button_Click(object sender, RoutedEventArgs e)
        {
            if (colorOpen)
            {
                colorOpen = false;

                Ce_Color.Visibility = Visibility.Hidden;
            }
            else
            {
                colorOpen = true;

                Ce_Color.Visibility = Visibility.Visible;
            }
        }
    }
}
