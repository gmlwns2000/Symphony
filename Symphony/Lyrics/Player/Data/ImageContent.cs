using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Symphony.Util;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Symphony.Lyrics
{
    public class ImageContent : IContent, IDisposable, IRemovable
    {
        public Resource Resource { get; set; }

        public BitmapScalingMode ScalingMode { get; set; } = BitmapScalingMode.Unspecified;
        public Stretch Stretch { get; set; } = Stretch.None;
        public double Width { get; set; } = -1;
        public double Height { get; set; } = -1;

        public ImageContent(Lyric Lyric)
        {
            Resource = new Resource(Lyric);
        }

        public override void Update()
        {
            if (Resource.IsExist)
            {
                Image img = new Image();
                
                BitmapImage bit = new BitmapImage();
                bit.BeginInit();
                bit.UriSource = new Uri(Resource.FilePath);
                bit.CacheOption = BitmapCacheOption.OnLoad;
                bit.CreateOptions = BitmapCreateOptions.None;
                bit.EndInit();

                img.Source = bit;

                img.HorizontalAlignment = HorizontalAlignment.Center;
                img.VerticalAlignment = VerticalAlignment.Center;
                img.UseLayoutRounding = UseLayoutRounding;

                if(Width >= 0)
                {
                    img.Width = Width;
                }
                if(Height >= 0)
                {
                    img.Height = Height;
                }

                img.Stretch = Stretch;
                RenderOptions.SetBitmapScalingMode(img, ScalingMode);

                Content = img;
            }
            else
            {
                Content = null;
            }
        }

        public void Remove()
        {
            Resource.Remove();
        }

        public void Dispose()
        {
            Resource.CloseStream();
        }
    }
}
