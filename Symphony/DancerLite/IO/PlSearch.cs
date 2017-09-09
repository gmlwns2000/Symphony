using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Dancer;
using Symphony.Server;
using System.IO;

namespace Symphony.DancerLite
{
    public static class PlSearch
    {
        public static PlotSearchResult Search(MusicMetadata meta)
        {
            bool localExist = false;
            string localPlPath = "";

            DirectoryInfo[] dis = new DirectoryInfo(PlotHelper.PlotDirectory).GetDirectories();
            List<MusicMetadata> Metadatas = new List<MusicMetadata>();
            List<string> plFiles = new List<string>();
            foreach(DirectoryInfo di in dis)
            {
                string plFile = Path.Combine(di.FullName, "pl.xml");
                if (File.Exists(plFile))
                {
                    plFiles.Add(plFile);
                    Metadatas.Add(new MusicMetadata(plFile));
                }
            }
            int index = ScoredSearcher.Search(Metadatas, meta);

            if(index > -1)
            {
                localExist = true;
                localPlPath = plFiles[index];
            }

            if (localExist)
            {
                return new PlotSearchResult(PlotExistState.LocalExist, null, localPlPath);
            }
            else
            {
                return new PlotSearchResult(PlotExistState.None);
            }
        }
    }
}
