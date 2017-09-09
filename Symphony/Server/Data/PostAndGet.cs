using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;

namespace Symphony.Server.Data
{
    public static class PostAndGet
    {
        private static CookieContainer cookieContainer = new CookieContainer();
        public static QueryResult Post(string uri, NameValueCollection postArgs)
        {
            HttpWebRequest req;

            try
            {
                req = (HttpWebRequest)WebRequest.Create(new Uri(uri)); // WebRequest 객체 형성 및 HttpWebRequest 로 형변환
                req.Timeout = 60000;
                req.CookieContainer = cookieContainer;

                if (postArgs != null && postArgs.Count > 0)
                {
                    req.Method = "POST";

                    StringBuilder sb = new StringBuilder();
                    foreach (string key in postArgs.Keys)
                    {
                        sb.Append(key).Append("=").Append(postArgs[key]).Append("&");
                    }
                    string data = sb.ToString(0, sb.Length - 1);

                    byte[] byteArray = Encoding.UTF8.GetBytes(data);

                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = byteArray.Length;

                    Stream dataStream = req.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                else
                {
                    req.Method = "GET";
                }

                req.ServicePoint.Expect100Continue = false;

                string strResult = null;

                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        Stream respPostStream = resp.GetResponseStream();
                        StreamReader readerPost = new StreamReader(respPostStream, Encoding.UTF8, true);

                        strResult = readerPost.ReadToEnd();
                    }
                    else
                    {
                        return new QueryResult(null, resp.ToString() + ": 에러", false);
                    }
                }

                return new QueryResult(null, strResult, true);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    var resp = (HttpWebResponse)ex.Response;
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        // 예외 처리
                        return new QueryResult(null, "서버의 파일을 찾을수 없습니다.", false);
                    }
                    else
                    {
                        // 예외 처리
                        return new QueryResult(null, resp.ToString() + ": 에러", false);
                    }
                }
                else
                {
                    // 예외 처리
                    return new QueryResult(null, ex.ToString(), false);
                }
            }
        }
    }
}
