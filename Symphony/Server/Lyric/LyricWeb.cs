using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Symphony.Server
{
    public class RegisteredLyric
    {
        public Song Song;
        public RegisteredData RawData;

        public string WebPath
        {
            get
            {
                if(RawData != null)
                {
                    return System.IO.Path.Combine(Connection.DIR_LyricFolder, RawData.Index.ToString()+".lyric");
                }
                else
                {
                    return "";
                }
            }
        }

        public RegisteredLyric(Song song, RegisteredData data)
        {
            this.Song = song;
            this.RawData = data;
        }
    }

    public class LyricWeb : IDisposable
    {
        public static string TableName = "sym_lyrics";
        public event EventHandler<ProgressUpdatedArgs> ProgressUpdated;
        
        private DataUploader dataUploader;

        public LyricWeb()
        {
            dataUploader = new DataUploader();
            dataUploader.ProgressUpdated += DataSearcher_ProgressUpdated;
        }

        private void DataSearcher_ProgressUpdated(object sender, ProgressUpdatedArgs e)
        {
            if(ProgressUpdated != null)
            {
                ProgressUpdated(sender, e);
            }
        }

        /// <summary>
        /// Search Lyric
        /// </summary>
        /// <returns>Tag = RegisteredLyric</returns>
        public static QueryResult Search(Song song)
        {
            QueryResult result = DataSearcher.Search(TableName, song.Index);

            if (result.Success)
            {
                if(result.Tag == null)
                {
                    return new QueryResult(result, "검색 결과가 없습니다.", false);
                }
                else
                {
                    return new QueryResult(result, "검색이 완료되었습니다.", true, new RegisteredLyric(song, ((RegisteredDataCollection)result.Tag).TargetData));
                }
            }
            else
            {
                return new QueryResult(result, result.Message, false);
            }
        }

        public void Dispose()
        {
            dataUploader.Dispose();

            ProgressUpdated = null;
        }

        /// <summary>
        /// Sign in on Mysql
        /// </summary>
        /// <returns>Tag = null</returns>
        public QueryResult Register(Song song, string lyricFile, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            QueryResult result = dataUploader.Upload(ServerDateType.Lyric, song.Index, lyricFile, token);

            return result;
        }
    }
}
