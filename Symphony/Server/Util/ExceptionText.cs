using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Util;
using System.Diagnostics;

namespace Symphony.Server
{
    public static class ExceptionText
    {
        public static string ToString(Exception e )
        {
            return e.ToString();
        }

        public static string ToString(int ErrorCode)
        {
            switch (ErrorCode)
            {
                case 1042:
                    return "서버가 오프라인입니다.";
                case 1062:
                    return "이미 사용중인 아이디입니다.";
                case 1064:
                    return "문법 오류입니다.";
                default:
                    return "알 수 없는 오류 코드: " + ErrorCode.ToString() + "\n운영자에게 연락 주세요";
            }
        }

        public static QueryResult PrintAndResult(string sender, Exception e)
        {
            int errorCode = -1;
            
            string errorTxt = ToString(errorCode);

            Debug.WriteLine("========\n" + e.ToString() + "\n\n" + errorTxt + "\n===========");

            return new QueryResult(null, errorTxt, false);
        }
    }
}
