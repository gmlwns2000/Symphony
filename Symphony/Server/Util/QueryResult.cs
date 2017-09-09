using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Server
{
    public class QueryResult
    {
        public DateTime TimeStamp { get; set; }
        public QueryResult Parent { get; set; }
        public string StackTrace
        {
            get
            {
                string ret = "StackTrace of (" + ToString() + ")\n";
                QueryResult q = Parent;

                while (true)
                {
                    if(q != null)
                    {
                        ret += q.ToString() + "\n";

                        q = q.Parent;
                    }
                    else
                    {
                        break;
                    }
                }

                return ret.Trim('\n');
            }
        }
        public string Message { get; set; }
        public bool Success { get; set; }
        public object Tag { get; set; }

        public QueryResult(QueryResult parent, string text, bool success)
        {
            TimeStamp = DateTime.Now;

            Message = text;
            Success = success;

            Logger.Log("QueryResult", ToString());
        }

        public QueryResult(QueryResult parent, string text, bool success, object tag)
        {
            TimeStamp = DateTime.Now;

            Message = text;
            Success = success;
            Tag = tag;

            Logger.Log("QueryResult", ToString());
        }

        public override string ToString()
        {
            return string.Format("[QueryResult: Messgae:({0}), Success:({1}), Tag:({2}), Parent:({3})]", Message, Success, Tag, Parent);
        }
    }
}
