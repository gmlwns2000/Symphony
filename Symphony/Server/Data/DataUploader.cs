using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Symphony.Server
{
    public class DataUploader:IDisposable
    {
        public event EventHandler<ProgressUpdatedArgs> ProgressUpdated;
        private CancellationToken token;

        public DataUploader()
        {

        }

        /// <summary>
        /// For NameValueCollection, id, type, name, file is Pre-Used, don't use them for post.
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="phpPath"></param>
        /// <param name="Names"></param>
        /// <returns></returns>
        public QueryResult Upload(ServerDateType type, int songId, string localData, CancellationToken token)
        {
            if (!Session.IsLogined)
            {
                return new QueryResult(null, "로그인을 먼저 해주세요", false);
            }

            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("id", "input");
            nvc.Add("type", "file");
            nvc.Add("name", "file");
            nvc.Add("user_index", Session.UserIndex.ToString());
            nvc.Add("song_index", songId.ToString());
            nvc.Add(Session.PHP_SessionIdName, Session.SessionID);

            string phpPath = "";
            switch (type)
            {
                case ServerDateType.Lyric:
                    phpPath = Connection.PHP_LyricUpload;
                    break;
                case ServerDateType.Plot:
                    break;
            }

            if(phpPath != "")
            {
                try
                {
                    return HttpUploadFile(phpPath, localData, nvc, songId);
                }
                catch (Exception e)
                {
                    Logger.Error(this, e);

                    return new QueryResult(null, "서버에 접속하던중 오류가 발생했습니다.", false);
                }
            }
            else
            {
                return new QueryResult(null, "지원되지 않는 타입입니다.", false);
            }
        }

        private QueryResult HttpUploadFile(string url, string file, NameValueCollection nvc, int songId)
        {
            Logger.Log(this, "start httpuploadfile");

            string paramName = "file";
            string contentType = "image/jpeg";

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);

            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {

                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                int uploaded = 0;
                int readed = 0;
                long total = fileStream.Length;

                Logger.Log(this, "start reading file http");
                Logger.Log(this, string.Format("Upload Info  length:{0}", fileStream.Length));

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    token.ThrowIfCancellationRequested();

                    rs.Write(buffer, 0, bytesRead);

                    uploaded += bytesRead;
                    readed += bytesRead;

                    if (uploaded == fileStream.Length || readed >= (double)total / 1000)
                    {
                        ProgressUpdated?.Invoke(this, new ProgressUpdatedArgs(((double)uploaded / total) * 10, 10, string.Format("파일을 업로드 하는중: {0}%", Math.Ceiling(((double)uploaded / total) * 1000) / 10)));

                        readed = 0;
                    }
                }

                fileStream.Close();
            }

            Logger.Log(this, "end reading file http");

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            string response = "";

            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                response = reader2.ReadToEnd();
            }
            catch (Exception ex)
            {
                Logger.Error(this, ex);

                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            if (response == "")
            {
                return new QueryResult(null, "서버에서 응답이 없습니다", false);
            }
            else
            {
                if (response.EndsWith("false"))
                {
                    if(response.EndsWith("userid false"))
                    {
                        Session.Logout();
                        return new QueryResult(null, "로그인 세션이 종료 되었습니다. 다시 로그인후 시도 해주십시오.", false, response);
                    }
                    else if (response.EndsWith("login error false"))
                    {
                        Session.Logout();
                        return new QueryResult(null, "로그인 데이터에 오류가 발생했습니다. 다시 로그인후 시도해주십시오.", false, response);
                    }
                    else
                    {
                        return new QueryResult(null, "서버에 파일을 업로드 하던 도중 오류가 발생했습니다.", false, response);
                    }
                }
                else if (response.EndsWith("true"))
                {
                    return new QueryResult(null, "업로드해주셔서 감사합니다.", true);
                }
                else
                {
                    return new QueryResult(null, "서버에서 오류가 발생했습니다.", false);
                }
            }
        }

        public void Dispose()
        {
            ProgressUpdated = null;
        }
    }
}
