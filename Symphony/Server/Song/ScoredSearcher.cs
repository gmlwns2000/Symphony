using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony;
using Symphony.Dancer;

namespace Symphony.Server
{
    public struct ServerExist
    {
        public bool Exist;
        public int Index;

        public ServerExist(bool Exist, int Index)
        {
            this.Exist = Exist;
            this.Index = Index;
        }
    }

    public static class ScoredSearcher
    {
        private static double Score(double max, string original, string key)
        {
            double score = 0;
            if (Util.TextTool.StringEmpty(original) && Util.TextTool.StringEmpty(key))
            {
                return 0;
            }

            if (original == null || Util.TextTool.StringEmpty(original))
            {
                original = "";
                return 0;
            }
            if (key == null || Util.TextTool.StringEmpty(key))
            {
                key = "";
                return 0;
            }

            if (original.ToLower() == key.ToLower())
            {
                return max;
            }
            else
            {
                string titleText = original;
                double appended = 0;
                while (appended < original.Length)
                {
                    if (titleText.ToLower() == key.ToLower())
                    {
                        score = (titleText.Length / (titleText.Length + appended)) * max;
                        break;
                    }
                    else if (titleText.ToLower().EndsWith(key.ToLower()))
                    {
                        score = (key.Length / original.Length) * max;
                        break;
                    }
                    else if (titleText.ToLower().StartsWith(key.ToLower()))
                    {
                        score = (key.Length / original.Length) * max;
                        break;
                    }

                    titleText.Remove(0, 1);
                    appended++;
                }
            }

            return Math.Max(max/10,score);
        }

        public static int Search(List<MusicMetadata> Database, MusicMetadata Keydata)
        {
            double[] score = new double[Database.Count];

            for (int i = 0; i < score.Length; i++) score[i] = 0;
            
            for (int i = 0; i < Database.Count; i++)
            {
                MusicMetadata meta = Database[i];
                score[i] += Score(50, Keydata.FileName, meta.FileName) + Score(50, meta.FileName, Keydata.FileName);
                score[i] += Score(50, Keydata.Title, meta.Title) + Score(50, meta.Title, Keydata.Title);
                score[i] += Score(12, Keydata.Artist, meta.Artist) + Score(12, meta.Artist, Keydata.Artist);
                score[i] += Score(24, Keydata.Album, meta.Album) + Score(24, meta.Album, Keydata.Album);
            }

            int maxIndex = -1;
            double maximum = -10000000;
            for (int i = 0; i < score.Length; i++)
            {
                if (score[i] >= maximum)
                {
                    maxIndex = i;
                    maximum = score[i];
                }
            }

            if (maximum >= 100) //파일 네임 혹은 제목이 일치 할때 리턴
            {
                return maxIndex;
            }

            return -1;
        }
    }
}
