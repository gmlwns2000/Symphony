using Symphony.Server;
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

namespace Symphony.Lyrics
{
    public class MetadataUpdated : EventArgs
    {
        public Server.MusicMetadata Metadata;
        public string Title
        {
            get
            {
                return Metadata.Title;
            }
        }
        public string Artist
        {
            get
            {
                return Metadata.Artist;
            }
        }
        public string Album
        {
            get
            {
                return Metadata.Album;
            }
        }
        public string FileName
        {
            get
            {
                return Metadata.FileName;
            }
        }
        public string Author
        {
            get
            {
                return Metadata.Author;
            }
        }

        public MetadataUpdated(Server.MusicMetadata Metadata)
        {
            this.Metadata = Metadata;
        }
    }

    /// <summary>
    /// MetadataEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MetadataEditor : UserControl
    {
        public event EventHandler<MetadataUpdated> Updated;
        MusicMetadata Metadata;
        bool inited = false;

        public MetadataEditor(MusicMetadata Metadata)
        {
            InitializeComponent();

            this.Metadata = Metadata;

            UpdateUI();
        }

        private void UpdateUI()
        {
            inited = false;

            Tb_Title.Text = Metadata.Title;
            Tb_Artist.Text = Metadata.Artist;
            Tb_Album.Text = Metadata.Album;
            Tb_FileName.Text = Metadata.FileName;
            Tb_Author.Text = Metadata.Author;

            inited = true;
        }

        private void UpdateMeta()
        {
            if (inited)
            {
                Metadata.Title = Tb_Title.Text;
                Metadata.Artist = Tb_Artist.Text;
                Metadata.Album = Tb_Album.Text;
                Metadata.FileName = Tb_FileName.Text;
                Metadata.Author = Tb_Author.Text;

                Updated?.Invoke(this, new MetadataUpdated(Metadata));
            }
        }

        private void Tb_Title_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMeta();
        }

        private void Tb_Album_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMeta();
        }

        private void Tb_Artist_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMeta();
        }

        private void Tb_FileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMeta();
        }

        private void Tb_Author_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMeta();
        }
    }
}
