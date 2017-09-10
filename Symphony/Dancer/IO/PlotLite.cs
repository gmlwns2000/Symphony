using Symphony.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Dancer
{
    public class PlotLite
    {
        public MusicMetadata Metadata { get; set; }
        public string PMXPath { get; set; }
        public string VMDPath { get; set; }
        public string WorkingDirectory { get; private set; }
        public string Version { get; set; }

        public PlotLite(MusicMetadata Metadata)
        {
            this.Metadata = Metadata;

            string dname = Dancer.PlotHelper.PlotName(Metadata);
            string path = Path.Combine(Dancer.PlotHelper.PlotDirectory, dname);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            WorkingDirectory = path;
        }

        public PlotLite(string plPath)
        {
            WorkingDirectory = Path.GetDirectoryName(plPath);

            PlLoader.Load(plPath, this);
        }

        public void PMXLoad(string pmxPath)
        {
            string pmxDir = Path.Combine(WorkingDirectory, "PMX");

            try
            {
                if (Directory.Exists(pmxDir))
                {
                    Directory.Delete(pmxDir, true);
                }
                Directory.CreateDirectory(pmxDir);
            }
            catch
            {

            }

            string groupName = Path.GetFileName(Path.GetDirectoryName(pmxPath));

            PMXPath = Path.Combine("PMX", groupName, Path.GetFileName(pmxPath));

            string dist = Path.Combine(pmxDir, groupName);

            Util.IO.DirectoryCopy(Path.GetDirectoryName(pmxPath), dist);
        }

        public void VMDLoad(string vmdPath)
        {
            string vmdDir = Path.Combine(WorkingDirectory, "VMD");

            try
            {
                if (Directory.Exists(vmdDir))
                {
                    Directory.Delete(vmdDir);
                }
                Directory.CreateDirectory(vmdDir);
            }
            catch
            {

            }

            string dist = Path.Combine(vmdDir, Path.GetFileName(vmdPath));

            VMDPath = Path.Combine("VMD", Path.GetFileName(vmdPath));

            File.Copy(vmdPath, dist, true);
        }
    }
}
