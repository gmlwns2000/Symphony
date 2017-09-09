using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace Symphony.Server
{
    public class RegisteredData
    {
        public int Index;
        public int SongId;
        public int UserId;
        public DateTime Time;

        public RegisteredData(int Index, int SongId, int UserId, DateTime Time)
        {
            this.Index = Index;
            this.SongId = SongId;
            this.UserId = UserId;
            this.Time = Time;
        }
    }

    public class RegisteredDataCollection : System.Collections.CollectionBase
    {
        public int SelectedIndex;
        public int SongIndex;
        public ServerSearchState SearchResult;
        public RegisteredData TargetData
        {
            get
            {
                if (SelectedIndex > -1 && SelectedIndex < List.Count)
                {
                    return Item(SelectedIndex);
                }
                else
                {
                    return null;
                }
            }
        }

        public RegisteredDataCollection(int index, int songIndex, ServerSearchState result)
        {
            this.SelectedIndex = index;
            this.SongIndex = songIndex;
            this.SearchResult = result;
        }

        public RegisteredDataCollection(int index)
        {
            this.SelectedIndex = index;
        }

        public void Add(RegisteredData data)
        {
            List.Add(data);
        }

        public RegisteredData Item(int index)
        {
            return (RegisteredData)List[index];
        }
    }

    public enum ServerDateType
    {
        Lyric = 0,
        Plot = 1
    }

    public class DataSearcher : IDisposable
    {
        public event EventHandler<ProgressUpdatedArgs> ProgressUpdated;

        public DataSearcher()
        {

        }

        /// <summary>
        /// Search Data in `tableName`
        /// Tag is Class:RegisterdData
        /// </summary>
        /// <returns>Tag = RegisteredDataCollection</returns>
        public static QueryResult Search(string tableName, int songId)
        {
            try
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("tablename", tableName);
                nvc.Add("songid", songId.ToString());

                QueryResult r = Data.PostAndGet.Post(Connection.PHP_DataSearch, nvc);

                if (r.Success)
                {
                    if (r.Message.EndsWith("false"))
                    {
                        return new QueryResult(r, "서버에서 작업중 오류가 발생했습니다.",false,r);
                    }
                    else
                    {
                        RegisteredDataCollection results = new RegisteredDataCollection(0);

                        JArray array = JArray.Parse(r.Message);
                        foreach (JObject itemObj in array)
                        {
                            int index = Convert.ToInt32(itemObj["id"].ToString());
                            int songid = Convert.ToInt32(itemObj["song_id"].ToString());
                            int userid = Convert.ToInt32(itemObj["user_id"].ToString());
                            DateTime dt = Convert.ToDateTime(itemObj["time"].ToString());
                            
                            RegisteredData song = new RegisteredData(index, songid, userid, dt);

                            results.Add(song);
                        }

                        if(results.Count > 0)
                        {
                            return new QueryResult(r, "", true, results);
                        }
                        else
                        {
                            return new QueryResult(r, "", true, null);
                        }
                    }
                }
                else
                {
                    return r;
                }
            }
            catch (Exception e)
            {
                return ExceptionText.PrintAndResult("DataSearcher.Search", e);
            }
        }

        private void UpadteMsg(double Value, double Maximum, string Status)
        {
            if(ProgressUpdated != null)
            {
                ProgressUpdated(this, new ProgressUpdatedArgs(Value, Maximum, Status));
            }

            Logger.Log(this, Status);
        }

        public void Dispose()
        {
            ProgressUpdated = null;
        }
    }
}
