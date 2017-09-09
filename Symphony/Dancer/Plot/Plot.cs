using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml;
using System.IO.Compression;
using System.Diagnostics;
using Symphony.Server;
using SlimDX;

namespace Symphony.Dancer
{
    /// <summary>
    /// 춤의 플롯을 노래와 연결할 메타데이터를 담고, 인스턴스를 가지고 있습니다.
    /// </summary>
    public class Plot
    {
        public string Version = "1";
        public bool Inited = false;

        public InstanceCollection Instances = new InstanceCollection();
        public Ratio Ratio;
        public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Center;
        public VerticalAlignment VerticalAlignment = VerticalAlignment.Center;
        public Vector3 CameraPosition;

        public MusicMetadata Metadata;

        public string WorkingDirectory;

        public Plot()
        {

        }

        /// <summary>
        /// 새로운 플롯을 생성합니다.
        /// </summary>
        /// <param name="WorkingDirectory"></param>
        /// <param name="Title"></param>
        /// <param name="Artist"></param>
        /// <param name="Album"></param>
        /// <param name="Author"></param>
        public Plot(MusicMetadata meta, Ratio Ratio, string workingDirectory = null)
        {
            this.Ratio = Ratio;

            Metadata = meta;

            if (workingDirectory == null)
            {
                WorkingDirectory = Path.Combine(PlotHelper.PlotDirectory, PlotHelper.PlotName(meta));
            }
            else
            {
                WorkingDirectory = workingDirectory;
            }

            DirectoryInfo di = new DirectoryInfo(WorkingDirectory);

            if (!di.Exists)
            {
                di.Create();
            }

            this.Inited = true;
        }
    }
}
