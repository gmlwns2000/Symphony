using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Symphony.Util;
using System.Diagnostics;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace Symphony.Server
{
    public class RegisteredSong
    {
        public string id { get; set; }
        public string filename { get; set; }
        public string title { get; set; }
        public string album { get; set; }
        public string artist { get; set; }
        public string create_time { get; set; }
        public string first_uploader { get; set; }
    }

    public class Song
    {
        public MusicMetadata metadata;
        public int Index;
        public DateTime CreationTime;
        public string FirstUploader;

        public Song(MusicMetadata metadata, int index, DateTime creationtime, string firstUploader)
        {
            this.metadata = metadata;
            this.Index = index;
            this.CreationTime = creationtime;
            this.FirstUploader = firstUploader;
        }
    }

    public enum ServerSearchState
    {
        Exist = 1,
        None = -1
    }

    public class SongCollection : System.Collections.CollectionBase
    {
        public int SelectedIndex;
        public MusicMetadata KeyData;
        public ServerSearchState SearchResult;
        public Song TargetSong
        {
            get
            {
                if(SelectedIndex > -1 && SelectedIndex < List.Count)
                {
                    return Item(SelectedIndex);
                }
                else
                {
                    return null;
                }
            }
        }

        public SongCollection(int index, MusicMetadata metadata, ServerSearchState result)
        {
            this.SelectedIndex = index;
            this.KeyData = metadata;
            this.SearchResult = result;
        }

        public SongCollection(int index)
        {
            this.SelectedIndex = index;
        }

        public void Add(Song song)
        {
            List.Add(song);
        }

        public Song Item(int index)
        {
            return (Song)List[index];
        }

        public List<MusicMetadata> ToMetadataList()
        {
            List<MusicMetadata> ret = new List<MusicMetadata>();

            foreach (object song in List)
            {
                ret.Add(((Song)song).metadata);
            }

            return ret;
        }
    }

    public static class SongSearcher
    {
        public static string TableName = "sym_songs";

        /// <summary>
        /// Search Song. Tag is SongCollection
        /// </summary>
        /// <returns>서버에서 노래를 찾든 말든 서버조회에 성공하면 true, 서버에서 노래를 찾지못햇을경우 Tag=null, 노래를 검색했지만 적당한 노래가 없을경우, SelectedIndex = -1</returns>
        public static QueryResult Search(MusicMetadata metadata)
        {
            try
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("filename", metadata.FileName);
                nvc.Add("title", metadata.Title);
                nvc.Add("artist", metadata.Artist);
                nvc.Add("album", metadata.Album);

                QueryResult r = Data.PostAndGet.Post(Connection.PHP_SongSearch, nvc);

                if (r.Success)
                {
                    if (r.Message.EndsWith("false"))
                    {
                        return new QueryResult(r, "서버에서 작업 도중 오류가 발생 했습니다.", false, r);
                    }
                    else
                    {
                        string strJson = r.Message;
                        
                        JArray array = JArray.Parse(strJson);

                        SongCollection results = new SongCollection(-1);

                        foreach (JObject itemObj in array)
                        {
                            int index = Convert.ToInt32(itemObj["id"].ToString());
                            string strDateTime = itemObj["create_time"].ToString();
                            string firstUploader = itemObj["first_uploader"].ToString();
                            DateTime dt = Convert.ToDateTime(strDateTime);

                            string artist = itemObj["artist"].ToString();
                            string album = itemObj["album"].ToString();
                            string filename = itemObj["filename"].ToString();
                            string title = itemObj["title"].ToString();

                            MusicMetadata meta = new MusicMetadata(title, artist, album, "", filename);
                            Song song = new Song(meta, index, dt, firstUploader);

                            results.Add(song);
                        }

                        QueryResult result = new QueryResult(r, "", true, null);
                        results.SearchResult = ServerSearchState.None;

                        //개수검사
                        if (results.Count > 0)
                        {
                            //검색시작
                            int index = ScoredSearcher.Search(results.ToMetadataList(), metadata);

                            //인댁스 검사
                            if (index >= 0)
                            {
                                results.SelectedIndex = index;
                                results.SearchResult = ServerSearchState.Exist;

                                result.Tag = results;

                                result.Message = "노래를 찾았습니다.";
                            }
                            else
                            {
                                result.Message = "노래를 찾을수없습니다.";
                            }
                        }
                        else
                        {
                            result.Message = "노래를 찾을수 없습니다.";
                        }

                        return result;
                    }
                }
                else
                {
                    return r;
                }
            }
            catch (Exception e)
            {
                return ExceptionText.PrintAndResult("SongSearcher.Search Metadata", e);
            }
        }

        /// <summary>
        /// RegisterSong Tag is Registerd Song. Time is just assumed. not correct
        /// </summary>
        /// <returns>Tag is Song</returns>
        public static QueryResult Register(MusicMetadata metadata)
        {
            if (!Session.IsLogined)
            {
                return new QueryResult(null, "로그인을 먼저 해주십시오.", false);
            }

            try
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("filename", metadata.FileName);
                nvc.Add("album", metadata.Album);
                nvc.Add("artist", metadata.Artist);
                nvc.Add("title", metadata.Title);
                nvc.Add(Session.PHP_SessionIdName, Session.SessionID);

                QueryResult r = Data.PostAndGet.Post(Connection.PHP_SongRegister, nvc);

                if (r.Success)
                {
                    if (r.Message.EndsWith("false"))
                    {
                        if(r.Message.EndsWith("login false"))
                        {
                            Session.Logout();

                            return new QueryResult(r, "로그인 세션이 만료 되었습니다. 다시 로그인 해주세요", false);
                        }
                        else
                        {
                            return new QueryResult(r, "서버에서 작업중 오류가 발생 했습니다.", false);
                        }
                    }
                    else
                    {
                        string[] spl = r.Message.Split('\n');

                        int lastid = Convert.ToInt32(spl[spl.Length - 2]);

                        return new QueryResult(r, "노래를 등록하는데 성공했습니다.", true, new Song(metadata, lastid, DateTime.UtcNow, Session.UserID));
                    }
                }
                else
                {
                    return r;
                }
            }
            catch (Exception e)
            {
                return ExceptionText.PrintAndResult("SongSearcher.RegisterSong", e);
            }
        }

        /// <summary>
        /// Search song, if isn't exist, Register then return Song just added.
        /// </summary>
        /// <returns>Tag is Song</returns>
        public static QueryResult SearchAndRegister(MusicMetadata metadata, int retry, int maxRetry)
        {
            QueryResult result = Search(metadata);

            if (result.Success)
            {
                if (result.Tag == null)
                {
                    Logger.Log("SongSearcher" ,"서버에서 노래를 찾지못함");

                    QueryResult rResult = Register(metadata);

                    Logger.Log("SongSearcher", "서버에 노래를 추가함");

                    if(rResult.Success && rResult.Tag != null)
                    {
                        return new QueryResult(rResult, "서버에 노래를 추가 했습니다.", true, rResult.Tag);
                    }
                    else
                    {
                        return new QueryResult(rResult, rResult.Message, false);
                    }
                }
                else
                {
                    Logger.Log("SongSearcher", "서버에서 노래를 찾음");

                    return new QueryResult(result, "", true, ((SongCollection)result.Tag).TargetSong);
                }
            }
            else
            {
                Logger.Log("SongSearcher", "서버에서 오류가 발생했습니다.");

                return new QueryResult(result, result.Message, false);
            }
        }
    }
}
